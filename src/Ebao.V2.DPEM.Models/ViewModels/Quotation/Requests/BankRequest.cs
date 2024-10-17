namespace Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;

public record BankRequest
{
    public string Bank { get; set; }
    public string Agency { get; set; }
    public string Account { get; set; }
    public string AccountDigit { get; set; }
}