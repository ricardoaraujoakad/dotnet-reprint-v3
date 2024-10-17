namespace Ebao.V2.DPEM.Api.Broker.Responses;

public record Broker(long Id, string Name, long AgreementId, int BrokerCatLis, int BrokerIdLis, int CommTypeIdLis, int RiskCatLis)
{
    public int CommCurType => 1;
    public double Comission { get; set; }
    public double RiskValue { get; set; }
    public double ShareRateLis { get; set; }
}