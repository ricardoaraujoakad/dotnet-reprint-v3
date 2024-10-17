namespace Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;

public record ClientRequest(long Id, string Name, string Identity, AddressRequest Address, bool Valid = true);