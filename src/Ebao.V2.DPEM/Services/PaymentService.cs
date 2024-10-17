using System.Globalization;
using System.Text.RegularExpressions;
using Ebao.V2.DPEM.Api.Payment;
using Ebao.V2.DPEM.Api.Payment.Requests;
using Ebao.V2.DPEM.Api.Payment.Responses;
using Ebao.V2.DPEM.Helpers;
using Ebao.V2.DPEM.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ebao.V2.DPEM.Services;

public class PaymentService : ServiceBase<PaymentService>
{
    private readonly IPaymentApi _paymentApi;
    private readonly UserService _userService;

    public PaymentService(ILogger<PaymentService> logger, IMemoryCache cache, RequestInfo requestInfo,
        IPaymentApi paymentApi, UserService userService) : base(logger, cache, requestInfo)
    {
        _paymentApi = paymentApi;
        _userService = userService;
    }

    public async ValueTask<V3PaymentResponse> SetQuotationAsPaidAsync(string quotationNo)
    {
        Logger.LogInformation("Efetuando pré-pagamento da cotação.");

        var syskeyPageToken = await GetSysKeyPageAsync();
        var paymentResponse = await SearchPaymentAsync(quotationNo, syskeyPageToken);

        paymentResponse.ReceiptNo = await SetQuotationAsPaidAsync(paymentResponse);

        Logger.LogInformation("Pré-pagamento efetuado com sucesso.");

        return paymentResponse;
    }

    private async ValueTask<string> SetQuotationAsPaidAsync(V3PaymentResponse response)
    {
        Logger.LogInformation("Efetuando pagamento da cotação {QuotationNo}.", response.ProposalNo);

        var request = new V3PaymentRequest(response, _userService.GetSyskeyRequestToken());
        var html = await _paymentApi.SetPaymentAsync(request);
        const string regex = @"Receipt No\. is (\w+)";
        var match = Regex.Match(html, regex);

        if (!match.Success)
            throw new InvalidOperationException("Não foi possível efetuar o pagamento.");

        var receiptNo = match.Groups[1].Value;
        Logger.LogInformation("Pagamento efetuado com sucesso. Número do recibo: {ReceiptNo}", receiptNo);
        return receiptNo;
    }

    private async ValueTask<long> GetSysKeyPageAsync()
    {
        Logger.LogInformation("Carregando página de pré-pagamento.");
        var html = await _paymentApi.LoadPrePaymentPageAsync();
        var reader = HtmlReader.CreateReader(html);
        var syskey = long.Parse(reader.GetValueFromXPathOrThrow("//*[@name='syskey_page_token']"));
        Logger.LogInformation("Página de pré-pagamento carregada com sucesso. Syskey: {Syskey}", syskey);
        return syskey;
    }

    private async ValueTask<V3PaymentResponse> SearchPaymentAsync(string quotationNo, long syskeyPageToken)
    {
        Logger.LogInformation("Buscando pagamento da cotação {QuotationNo}.", quotationNo);

        var request =
            new V3SearchPremiumActionRequest(quotationNo, _userService.GetSyskeyRequestToken(), syskeyPageToken);

        var html = await _paymentApi.SearchPaymentAsync(request);
        var reader = HtmlReader.CreateReader(html);

        var node = reader.GetNodeByName("checkVO");

        if (node == null)
            throw new NullReferenceException("Pagamento não encontrado.");

        var code = node.GetAttributeValue("payorcustcode", "");

        Logger.LogInformation("Pagamento encontrado. Código: {Code}", code);

        var policyId = node.GetAttributeValue("policyId", "");
        var amountText = node.ParentNode.ParentNode.ChildNodes.ElementAt(13).InnerText;

        if (string.IsNullOrEmpty(amountText))
            throw new NullReferenceException("Valor do pagamento não encontrado.");

        amountText = amountText.Replace(",", ".").Trim();

        var amount = decimal.Parse(amountText, CultureInfo.InvariantCulture);
        var syskeyPage = long.Parse(reader.GetValueByNameOrThrow("syskey_page_token"));

        return new V3PaymentResponse(amount, code, policyId, quotationNo, syskeyPage);
    }
}