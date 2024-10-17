using Ebao.V2.DPEM.Api.Data.Responses;
using Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;
using Refit;
using static Ebao.V2.DPEM.Api.Client.Requests.v3IdentityType;

namespace Ebao.V2.DPEM.Api.Client.Requests;

public record V3CreateClientRequest
{
    [AliasAs("customerType")]
    public string CustomerType
    {
        get
        {
            switch (IdentityType)
            {
                case (int)CPF:
                    return "ind";
                case (int)CNPJ:
                    return "org";
                default:
                    throw new Exception("Tipo de cliente não suportado.");
            }
        }
    }

    [AliasAs("sActionType")]
    public string SActionType => "saveNewCustomer";

    [AliasAs("fromUrl")]
    public string FromUrl => "API.gs.pol.nb.action.NewBizAction?operId=LoaderCustInfo";

    [AliasAs("isEndo")]
    public bool IsEndo => false;

    [AliasAs("telephone")]
    public string Telephone => $"{AreaCode},{Phone},"; //O eBao envia a última vírgula também para separar os campos. 

    [AliasAs("entryMode")]
    public string EntryMode => "add";

    [AliasAs("name03")]
    public string FirstName { get; }

    [AliasAs("name04")]
    public string LastName { get; }

    [AliasAs("name01")]
    public string SocialFirstName { get; set; }

    [AliasAs("name02")]
    public string SocialLastName { get; set; }

    [AliasAs("idNumber")]
    public string Identity { get; set; }

    [AliasAs("idType")]
    public int IdentityType { get; set; }

    [AliasAs("birthDayStr")]
    public string BirthDate { get; } = String.Empty;

    [AliasAs("businessTelephone_areaCode")]
    public int AreaCode { get; }

    [AliasAs("businessTelephone_phone")]
    public ulong Phone { get; }

    [AliasAs("prospectCustomer")]
    public int ProspectCustomer => 1;

    [AliasAs("countryOfBirth")]
    public int CountryOfBirth => 76;

    [AliasAs("nationality")]
    public int Nationality => 26;

    [AliasAs("indiPtyType")]
    public int IndiPtyType => 2;

    [AliasAs("ptyStatusDate")]
    public string UpdateDate { get; }

    [AliasAs("regionLv1")]
    public int CountryId => 130; //130 é Brasil

    [AliasAs("regionLv1firstLoad")]
    public bool RegionLv1FirstLoad => false;

    [AliasAs("regionLv2")]
    public int StateId { get; }

    [AliasAs("regionLv2firstLoad")]
    public bool RegionLv2FirstLoad => false;

    [AliasAs("address03")]
    public int StreetTypeId => 86; //86 é o tipo Rua.

    [AliasAs("address03firstLoad")]
    public bool Address03FirstLoad => false;

    [AliasAs("address04")]
    public string Street { get; }

    [AliasAs("address02")]
    public string District { get; }

    [AliasAs("address01")]
    public string City { get; }

    [AliasAs("address07")]
    public string ZipCode { get; }

    [AliasAs("address05")]
    public string Number { get; set; }

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    [AliasAs("address06")]
    public string Complement { get; set; }

    public V3CreateClientRequest(InsuredRequest insuredInfo, int areaCode, ulong phone, V3AddressResponse address,
        string syskeyRequestToken)
    {
        IdentityType = insuredInfo.Identity.Length == 14 ? (int)CNPJ : (int)CPF;
        FirstName = insuredInfo.FirstName;
        LastName = insuredInfo.LastName;
        SocialFirstName = insuredInfo.SocialFirstName;
        SocialLastName = insuredInfo.SocialLastName;
        Identity = insuredInfo.Identity;
        BirthDate = insuredInfo.BirthDate.ToString("dd/MM/yyyy");
        AreaCode = areaCode;
        Phone = phone;
        UpdateDate = DateTime.Now.ToString("dd/MM/yyyy");
        StateId = address.StateId;
        Street = address.Street;
        District = address.District;
        City = address.City;
        ZipCode = address.ZipCode;
        Number = insuredInfo.Address.Number;
        SyskeyRequestToken = syskeyRequestToken;
        Complement = insuredInfo.Address.Complement;
    }
}