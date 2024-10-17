using Refit;

namespace Ebao.V2.DPEM.Api.Payment.Requests;

public class V3SearchPremiumActionRequest
{
    [AliasAs("currencyType")]
    public int CurrencyType => 28;
    
    [AliasAs("searchCurrency")]
    public int searchCurrency => 28;

    [AliasAs("proposalNo")]
    public string QuotationNo { get; set; }

    [AliasAs("receiptDate")]
    public string ReceiptDate => DateTime.Now.ToString("dd/MM/yyyy");

    [AliasAs("amount")]
    public int Amount => 0;
    
    [AliasAs("sumPayAmount")]
    public int SumPayAmount => 0;
    
    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; set; }
    
    [AliasAs("syskey_page_token")]
    public long SyskeyPageToken { get; set; }

    public V3SearchPremiumActionRequest(string quotationNo, string syskeyRequestToken, long syskeyPageToken)
    {
        QuotationNo = quotationNo;
        SyskeyRequestToken = syskeyRequestToken;
        SyskeyPageToken = syskeyPageToken;
    }
}