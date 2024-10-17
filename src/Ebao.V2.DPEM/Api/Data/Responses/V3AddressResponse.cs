namespace Ebao.V2.DPEM.Api.Data.Responses;

public class V3AddressResponse
{
    public int StateId { get; set; }
    public string Street { get; set; }
    public string District { get; set; }
    public string City { get; set; }
    public string ZipCode { get; set; }
    public string Number { get; set; }
    public string Complement { get; set; }

    // Construtor opcional para inicializar a classe
    public V3AddressResponse(int stateId, string street, string district, string city, string zipCode, string number, string complement)
    {
        StateId = stateId;
        Street = street;
        District = district;
        City = city;
        ZipCode = zipCode;
        Number = number;
        Complement = complement;
    }

    // Construtor padrão opcional
    public V3AddressResponse()
    {
    }
}