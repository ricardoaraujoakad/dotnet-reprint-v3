using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public class V3IssueRequest
{
    [AliasAs("docDecision")]
    public string DocDecision => "close";

    [AliasAs("transaction")]
    public string Transaction => "SP Policy issuance";

    [AliasAs("operId")]
    public string OperId => "QB_ISSUE";

    [AliasAs("transition")]
    public string Transition => OperId;

    [AliasAs("transactionType")]
    public string TransactionType => "Issue";

    [AliasAs("rejCode")]
    public int RejCode => 3;
    
    [AliasAs("issuedAs")]
    public int IssuedAs => 2;

    [AliasAs("policyCate")]
    public int PolicyCate => 1;

    [AliasAs("generatePolicyDocFlagValue")]
    public int GeneratePolicyDocFlagValue => 1;
    
    [AliasAs("generatePolicyDocFlag")]
    public int GeneratePolicyDocFlag => 1;
    
    [AliasAs("compareTo")]
    public int CompareTo => 1;
    
    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    public V3IssueRequest(string syskeyRequestToken)
    {
        SyskeyRequestToken = syskeyRequestToken;
    }
}