namespace Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;

public class InsuredRequest
{
    public string FirstName { get; set; }
    public string LastName { get; set; }

    public string SocialFirstName { get; set; } = "";
    public string SocialLastName { get; set; } = "";
    
    public DateTime BirthDate { get; set; }
    public string Phone { get; set; }
    public string Identity { get; set; }
    public AddressRequest Address { get; set; }

    public InsuredRequest(string firstName, string lastName, DateTime birthDate, string phone, string identity,
        AddressRequest address)
    {
        FirstName = firstName;
        LastName = lastName;
        BirthDate = birthDate;
        Phone = phone;
        Identity = identity;
        Address = address;
    }

    public InsuredRequest()
    {
    }
}