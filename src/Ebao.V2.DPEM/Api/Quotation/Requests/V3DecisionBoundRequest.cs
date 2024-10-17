using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public class V3DecisionBoundRequest
{
    [AliasAs("operId")]
    public string OperId => "LoaderMPDecision";

    //Sumbit não é erro de digitação!!!
    [AliasAs("isFrameSumbit")]
    public int IsFrameSumbit => 1;
    
    [AliasAs("selVersion")]
    public int SelVersion => 1;
    
    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    public V3DecisionBoundRequest(string syskeyRequestToken)
    {
        SyskeyRequestToken = syskeyRequestToken;
    }
}