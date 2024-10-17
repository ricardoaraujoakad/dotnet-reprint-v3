namespace Ebao.V2.DPEM.Models.Template;

public class SearchQuotationTemplate
{
    public string QuoteNo { get; set; }

    public SearchQuotationTemplate(string quoteNo)
    {
        QuoteNo = quoteNo;
    }
}