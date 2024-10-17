using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public class V3SearchQuotationRequest
{
    [AliasAs("wfPageType")]
    public string WfPageType => "worklist";

    [AliasAs("procName")]
    public string ProcName => "NewBiz Process";

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    [AliasAs("key_wincony_xmlData")]
    public string Xml { get; }

    public V3SearchQuotationRequest(string xml, string syskeyRequestToken)
    {
        SyskeyRequestToken = syskeyRequestToken;
        Xml = xml;
    }
}