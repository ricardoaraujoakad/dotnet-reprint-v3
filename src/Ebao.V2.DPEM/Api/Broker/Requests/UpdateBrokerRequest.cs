using Refit;

namespace Ebao.V2.DPEM.Api.Broker.Requests;

public record UpdateBrokerRequest
{
    [AliasAs("shareRateLis")]
    public double Comission { get; init; }

    [AliasAs("agentCode")]
    public long Id { get; init;}

    [AliasAs("agentName")]
    public string Name { get; init;}

    [AliasAs("agreementId")]
    public long AgreementId { get; init;}

    [AliasAs("brokerCatLis")]
    public int BrokerCatLis { get; init;}

    [AliasAs("brokerIdLis")]
    public int BrokerIdLis { get; init;}

    [AliasAs("commCurType")]
    public int CommCurType { get; init;}

    [AliasAs("commTypeIdLis")]
    public int CommTypeIdLis { get; init;}

    [AliasAs("defaultRiskValueLis")]
    public double RiskValue { get;init; }

    [AliasAs("operId")]
    public string OperId => "SaverDECommInfo";

    [AliasAs("riskCatLis")]
    public int RiskCatLis { get; init;}

    [AliasAs("adjustCommRateLis")]
    public double ShareRateLis { get;init; } 

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; init;}
}