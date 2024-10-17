using Ebao.V2.DPEM.Models.Contants;
using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public record V3WorklistRequest(string SyskeyRequestToken)
{
    [AliasAs("key_wincony_xmlData")]
    public string Xml { get; set; } = string.Empty;

    [AliasAs("operationType")]
    public string OperationType { get; set; } = string.Empty;

    [AliasAs("pageno")]
    public int Pageno { get; set; } = 1;

    [AliasAs("pageno")]
    public int PageNo2 { get; set; } = 1;

    [AliasAs("procName")]
    public string ProcName { get; set; } = "NewBiz Process";

    [AliasAs("searchByButton")]
    public bool SearchByButton { get; set; } = true;

    [AliasAs(Parameters.Syskey)]
    public string SyskeyRequestToken { get; set; } = SyskeyRequestToken;

    [AliasAs("wfPageType")]
    public string WfPageType { get; set; } = "worklist";
}