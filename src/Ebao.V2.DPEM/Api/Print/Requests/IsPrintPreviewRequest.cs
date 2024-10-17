using Refit;

namespace Ebao.V2.DPEM.Api.Print.Requests;

public class IsPrintPreviewRequest
{
    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; set; } = string.Empty;

    [AliasAs("actionType")]
    public string ActionType { get; set; } = "isPrintPreview";

    [AliasAs("productCode")]
    public string ProductCode { get; set; } = string.Empty;

    [AliasAs("transactionType")]
    public string TransactionType { get; set; } = "Issue";
}
