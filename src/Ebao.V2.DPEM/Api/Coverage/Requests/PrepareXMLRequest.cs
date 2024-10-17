using Refit;

namespace Ebao.V2.DPEM.Api.Coverage.Requests;

public class PrepareXMLRequest
{
    [AliasAs("indexParams")]
    public string IndexParams { get; set; } = "{}";
    
    [AliasAs("insuredId")]
    public string InsuredId { get; set; }

    [AliasAs("key_wincony_xmlData")]
    public string Xml { get; set; }

    [AliasAs("pageId")]
    public string PageId { get; set; }

    [AliasAs("sActionType")]
    public string Action => "copyShareSIToCT";

    public PrepareXMLRequest(string xml, string pageId)
    {
        Xml = xml;
        PageId = pageId;
    }
}