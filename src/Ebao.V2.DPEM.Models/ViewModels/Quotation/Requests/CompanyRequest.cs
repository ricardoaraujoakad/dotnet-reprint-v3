namespace Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;

public record CompanyRequest
{
    public string Name { get; set; }
    public string SusepCode { get; set; }
    public double Commission { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Identity { get; set; }
    public AddressRequest Address { get; set; }
    public BankRequest BankInfo { get; set; }
}