using Ebao.V2.DPEM.Api.Broker;
using Ebao.V2.DPEM.Api.Broker.Requests;
using Ebao.V2.DPEM.Api.Broker.Responses;
using Ebao.V2.DPEM.Helpers;
using Ebao.V2.DPEM.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ebao.V2.DPEM.Services;

public class BrokerService : ServiceBase<BrokerService>
{
    private readonly UserService _userService;
    private readonly IBrokerApi _brokerApi;

    public BrokerService(ILogger<BrokerService> logger, IMemoryCache cache, RequestInfo requestInfo,
        IBrokerApi brokerApi, UserService userService) : base(logger, cache, requestInfo)
    {
        _brokerApi = brokerApi;
        _userService = userService;
    }

    public async ValueTask<Broker> GetBrokerInfoAsync()
    {
        Logger.LogInformation("Obtendo informações da corretora.");

        var body = new LoadPageBrokerRequest(_userService.GetSyskeyRequestToken());
        var pageHtml = await _brokerApi.LoadPageBrokerAsync(body);

        var reader = new HtmlReader();
        reader.Load(pageHtml);

        var id = long.Parse(reader.GetValueByNameOrThrow("agentCode"));
        var name = reader.GetValueByNameOrThrow("agentName");
        var comission = double.Parse(reader.GetValueByNameOrThrow("shareRateLis"));
        var riskValue = double.Parse(reader.GetValueByNameOrThrow("defaultRiskValueLis"));
        var agreementId = long.Parse(reader.GetValueByNameOrThrow("agreementId"));
        var brokerCatLis = int.Parse(reader.GetValueByNameOrThrow("brokerCatLis"));
        var brokerIdLis = int.Parse(reader.GetValueByNameOrThrow("brokerIdLis"));
        var commTypeIdLis = int.Parse(reader.GetValueByNameOrThrow("commTypeIdLis"));
        var riskCatLis = int.Parse(reader.GetValueByNameOrThrow("riskCatLis"));
        var shareRateLis = double.Parse(reader.GetValueByNameOrThrow("adjustCommRateLis"));

        var broker = new Broker(id, name, agreementId, brokerCatLis, brokerIdLis, commTypeIdLis, riskCatLis)
        {
            ShareRateLis = shareRateLis,
            Comission = comission,
            RiskValue = riskValue
        };

        Logger.LogInformation("Broker information retrieved: {@Broker}", broker);

        return broker;
    }

    private async ValueTask UpdateBrokerAsync(Broker broker)
    {
        Logger.LogInformation("Atualizando informações da corretora. {@Broker}", broker);

        var requestBody = new UpdateBrokerRequest
        {
            Id = broker.Id,
            Name = broker.Name,
            Comission = broker.Comission,
            RiskValue = broker.RiskValue,
            AgreementId = broker.AgreementId,
            BrokerCatLis = broker.BrokerCatLis,
            BrokerIdLis = broker.BrokerIdLis,
            CommCurType = broker.CommCurType,
            RiskCatLis = broker.RiskCatLis,
            ShareRateLis = broker.ShareRateLis,
            CommTypeIdLis = broker.CommTypeIdLis,
            SyskeyRequestToken = _userService.GetSyskeyRequestToken()
        };

        await _brokerApi.UpdateBrokerAsync(requestBody);

        Logger.LogInformation("Informações da corretora atualizadas com sucesso.");
    }

    public async ValueTask SetBrokerComissionAsync(double comission)
    {
        Logger.LogInformation($"Atualizando informações de comissão da corretora. Valor: {comission}");
        var broker = await GetBrokerInfoAsync();

        if (broker.ShareRateLis != comission || broker.Comission != comission)
        {
            broker.Comission = comission;
            broker.ShareRateLis = comission;
            broker.RiskValue = comission;
            await UpdateBrokerAsync(broker);
        }
        else
        {
            Logger.LogInformation("Não há comissão a ser atualizada.");
        }
    }
}