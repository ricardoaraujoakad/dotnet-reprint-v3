using Refit;

namespace Ebao.V2.DPEM.Api.Broker.Requests;

public record LoadPageBrokerRequest
{
    public LoadPageBrokerRequest(string syskeyRequestToken)
    {
        SyskeyRequestToken = syskeyRequestToken;
    }

    [AliasAs("operId")]
    public string OperId => "SaverDERIFactor";

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }
}