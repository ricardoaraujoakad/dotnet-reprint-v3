using Refit;

namespace Ebao.V2.DPEM.Api.Coverage.Requests;

public class AddCoverageRequest
{
    [AliasAs("_ctId")]
    public string Id { get; }


    [AliasAs("_insuredCateCode")]
    public int InsuredCateCode { get; set; } = 4;

    [AliasAs("ctx.bizProductCode")]
    public string CtxBizProductCode { get; set; } = Config.ProductCode;

    [AliasAs("ctx.ctCode")]
    public string CtxCtCode { get; set; } = "null";

    [AliasAs("ctx.ctId")]
    public string CtxCtId => Id;

    [AliasAs("ctx.insuredCateCode")]
    public int CtxInsuredCateCode { get; set; } = 0;

    [AliasAs("ctx.insuredId")]
    public int CtxInsuredId { get; set; }

    [AliasAs("ctx.isEndorsement")]
    public string CtxIsEndorsement { get; set; } = "false";

    [AliasAs("ctx.isInDataEntry")]
    public string CtxIsInDataEntry { get; set; } = "true";

    [AliasAs("ctx.isInManualPricing")]
    public string CtxIsInManualPricing { get; set; } = "false";

    [AliasAs("ctx.isInQuotation")]
    public string CtxIsInQuotation { get; set; } = "false";

    [AliasAs("ctx.isNewBiz")]
    public string CtxIsNewBiz { get; set; } = "true";

    [AliasAs("ctx.isRenewal")]
    public string CtxIsRenewal { get; set; } = "false";

    [AliasAs("ctx.policyCtId")]
    public int CtxPolicyCtId { get; set; } = 0;

    [AliasAs("ctx.policyId")]
    public string CtxPolicyId => PolicyId;

    [AliasAs("ctx.productCode")]
    public string CtxProductCode => CtxBizProductCode;

    [AliasAs("ctx.productId")]
    public string CtxProductId { get; set; } = Config.ProductId;

    /// <summary>
    /// Não mapeado
    /// </summary>
    [AliasAs("fromGroup")]
    public string FromGroup { get; set; } = string.Empty;

    /// <summary>
    /// Não mapeado
    /// </summary>
    [AliasAs("fromInsuredGroup")]
    public string FromInsuredGroup { get; set; } = string.Empty;

    /// <summary>
    /// Não mapeado
    /// </summary>
    [AliasAs("groupId")]
    public string GroupId { get; set; } = string.Empty;

    /// <summary>
    /// Não mapeado
    /// </summary>
    [AliasAs("insuredId")]
    public string InsuredId { get; set; } = string.Empty;

    [AliasAs("key_wincony_xmlData")]
    public string Xml { get; } = string.Empty;

    [AliasAs("needSave")]
    public int NeedSave { get; set; } = 1;

    [AliasAs("nextPageType")]
    public int NextPageType { get; set; } = 1;

    [AliasAs("pageType")]
    public int PageType { get; set; } = 3;

    /// <summary>
    /// Não mapeado
    /// </summary>
    [AliasAs("planIdOfLisig")]
    public string PlanIdOfLisig { get; set; } = string.Empty;

    /// <summary>
    /// Não mapeado
    /// </summary>
    [AliasAs("policyCtId")]
    public string PolicyCtId { get; set; } = string.Empty;

    [AliasAs("policyId")]
    public string PolicyId { get; }

    [AliasAs("prePageType")]
    public int PrePageType { get; set; } = 1;

    [AliasAs("sActionType")]
    public string SActionType { get; set; } = "doDefault";

    [AliasAs("sectionName")]
    public string SectionName { get; set; } = string.Empty;

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    [AliasAs("syskey_page_token")]
    public long SyskeyPageToken { get; }

    public AddCoverageRequest(string id, string policyId, string xml, string syskeyRequestToken, long syskeyPageToken)
    {
        Id = id;
        PolicyId = policyId;
        SyskeyRequestToken = syskeyRequestToken;
        SyskeyPageToken = syskeyPageToken;
        Xml = xml;
    }
}