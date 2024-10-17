using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public class V3DispatchRequest
{
    [AliasAs("loaderBizPrdtCode")]
    public string LoaderBizPrdtCode => Config.ProductCode;

    [AliasAs("loaderInstanceId")]
    public int LoaderInstanceId => 0;

    [AliasAs("loaderPolicyId")]
    public string PolicyId { get; }

    [AliasAs("loaderProductCode")]
    public string LoaderProductCode => Config.ProductCode;

    [AliasAs("loaderProductId")]
    public string LoaderProductId => Config.ProductId;

    [AliasAs("operId")]
    public string OperId => "DispatchQB";

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; set; }

    public V3DispatchRequest(string policyId, string syskeyRequestToken)
    {
        PolicyId = policyId;
        SyskeyRequestToken = syskeyRequestToken;
    }
}