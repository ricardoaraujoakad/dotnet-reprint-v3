using Refit;

namespace Ebao.V2.DPEM.Api.Quotation.Requests;

public class V3QuickQuoteRequest(long SyskeyPageToken, string SyskeyRequestToken)
{
    [AliasAs("decision")]
    public string Decision { get; set; } = "";  

    //TODO: Verificar possibilidade de remoção
    [AliasAs("dropdown_proCodetxt")]
    public string DropdownProCodetxt { get; set; } = "DPEM-Danos Pessoais Causados por Embarcação";

    [AliasAs("moduleRadio")]
    public string ModuleRadio { get; set; } = "QuickQuote";

    [AliasAs("operId")]
    public string OperId { get; set; } = "SubmitNBQuickQuoteProcode";

    [AliasAs("proCode")]
    public string ProductId { get; set; } = Config.ProductId;

    [AliasAs("result1radio")]
    public string Result1Radio { get; set; } = "singlePolicy";

    [AliasAs("syskey_page_token")]
    public long SyskeyPageToken { get; set; } = SyskeyPageToken;

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; set; } = SyskeyRequestToken;
}