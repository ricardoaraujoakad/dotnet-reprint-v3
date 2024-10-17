using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public class V3QuoteBoundRequest
{
    [AliasAs("compareTo")]
    public int CompareTo => 1;

    [AliasAs("operId")]
    public string OperId => "MP_TO_QB";

    [AliasAs("transition")]
    public string Transition => OperId;

    [AliasAs("policyCate")]
    public int PolicyCate => 1;

    [AliasAs("generatePolicyDocFlagValue")]
    public int GeneratePolicyDocFlagValue => 1;

    [AliasAs("rejCode")]
    public int RejCode => 3;

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    public V3QuoteBoundRequest(string syskeyRequestToken)
    {
        SyskeyRequestToken = syskeyRequestToken;
    }
}