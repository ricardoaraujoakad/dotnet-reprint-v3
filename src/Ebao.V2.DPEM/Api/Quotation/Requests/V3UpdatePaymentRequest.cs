using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public class V3UpdatePaymentRequest
{
    [AliasAs("operId")]
    public string OperId => "SaverDEPayment";

    [AliasAs("payerCode")]
    public string PayerCode { get; }

    [AliasAs("payMode")]
    public int PayMode => 300;

    [AliasAs("deltaIapp")]
    public decimal Amount { get; set; }

    [AliasAs("splitTotalAmount")]
    public decimal SplitTotalAmount => Amount;

    [AliasAs("premiumPayable")]
    public decimal PremiumPayable => Amount;

    [AliasAs("payerRoleType")]
    public int PayerRoleType { get; }

    [AliasAs("bpRadio")]
    public string BpRadio { get; }

    [AliasAs("dueDate")]
    public string DueDate { get; set; }

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    public V3UpdatePaymentRequest(string payerCode, decimal amount, string bpRadio, DateTime dueDate,
        string syskeyRequestToken, bool isCompany)
    {
        PayerCode = payerCode;
        Amount = amount;
        BpRadio = bpRadio;
        SyskeyRequestToken = syskeyRequestToken;
        DueDate = dueDate.ToString("dd/MM/yyyy");
        PayerRoleType = isCompany ? 21 : 20;
    }
}