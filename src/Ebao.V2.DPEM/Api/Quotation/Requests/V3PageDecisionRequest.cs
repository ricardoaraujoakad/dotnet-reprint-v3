using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public record V3PageDecisionRequest
{
    public V3PageDecisionRequest(string syskeyRequestToken)
    {
        SyskeyRequestToken = syskeyRequestToken;
    }
    
    [AliasAs("operId")]
    public string OperId => "SaverDERIFactor";
    
    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }
}