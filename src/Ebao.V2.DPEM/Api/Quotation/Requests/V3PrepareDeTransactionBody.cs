using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public record V3PrepareDeTransactionBody
{
    [AliasAs("loaderBizPrdtCode")]
    public string LoaderBizPrdtCode => Config.ProductCode;

    [AliasAs("loaderInstanceId")]
    public int LoaderInstanceId => 0;

    [AliasAs("loaderPolicyId")]
    public string PolicyId { get; set; }

    [AliasAs("loaderProductCode")]
    public string LoaderProductCode => Config.ProductCode;

    [AliasAs("loaderProductId")]
    public string LoaderProductId => Config.ProductId;

    [AliasAs("operId")]
    public string OperId => "DEDispatchMP";

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; set; }

    public V3PrepareDeTransactionBody(string policyId, string syskeyRequestToken)
    {
        PolicyId = policyId;
        SyskeyRequestToken = syskeyRequestToken;
    }
}