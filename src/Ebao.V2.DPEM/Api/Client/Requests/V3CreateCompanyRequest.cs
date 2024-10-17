using Ebao.V2.DPEM.Api.Data.Responses;
using Ebao.V2.DPEM.Models.ViewModels.Quotation.Requests;
using Refit;

namespace Ebao.V2.DPEM.Api.Client.Requests;

public class V3CreateCompanyRequest
{
    [AliasAs("customerType")]
    public string CustomerType => "company";

    [AliasAs("sActionType")]
    public string SActionType => "saveNewCustomer";

    [AliasAs("fromUrl")]
    public string FromUrl => "API.gs.pol.nb.action.NewBizAction?operId=LoaderCustInfo";

    [AliasAs("isEndo")]
    public string IsEndo { get; } = "false";

    [AliasAs("telephone")]
    public string Telephone => $"{AreaCode},{Phone},"; //O eBao envia a última vírgula também para separar os campos. 

    [AliasAs("entryMode")]
    public string EntryMode => "add";

    [AliasAs("partyName")]
    public string PartyName { get; set; }

    [AliasAs("organizationIdType")]
    public int OrganizationIdType => 1;

    [AliasAs("registerNumber")]
    public string RegisterNumber { get; set; }

    [AliasAs("abbrName")]
    public string RegisterName { get; set; }

    [AliasAs("businessTelephone_areaCode")]
    public int AreaCode { get; }

    [AliasAs("businessTelephone_phone")]
    public ulong Phone { get; }

    [AliasAs("businessTelephone_extension")]
    public string PhoneExtension { get; } = "";

    [AliasAs("fax")]
    public string Fax { get; set; } = "";

    [AliasAs("email")]
    public string Email { get; set; } = "";

    [AliasAs("gstRegNo")]
    public string GstRegNo { get; set; } = "";

    [AliasAs("prospectCustomer")]
    public int ProspectCustomer => 1;

    [AliasAs("contactType")]
    public int ContactType => 1;

    [AliasAs("orgPtyType")]
    public int OrgPtyType => 1;

    [AliasAs("field01")]
    public string Field01 => "N";

    [AliasAs("monthIncomeRange")]
    public string MonthIncomeRange { get; set; } = "";

    [AliasAs("ptyStatusDate")]
    public string UpdateDate { get; }

    [AliasAs("regionLv1")]
    public int CountryId => 130; //130 é Brasil

    [AliasAs("regionLv1firstLoad")]
    public string RegionLv1FirstLoad => "false";

    [AliasAs("regionLv2")]
    public int StateId { get; }

    [AliasAs("regionLv2firstLoad")]
    public string RegionLv2FirstLoad => "false";

    [AliasAs("address03")]
    public int StreetTypeId => 86; //86 é o tipo Rua.

    [AliasAs("address03firstLoad")]
    public string Address03FirstLoad => "false";

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

    [AliasAs("address06")]
    public string Complement { get; set; }

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    // Novos campos adicionados
    [AliasAs("displayBack")]
    public string DisplayBack { get; set; } = "";

    [AliasAs("policyId")]
    public string PolicyId { get; set; } = "";

    [AliasAs("legalStatus")]
    public string LegalStatus { get; set; } = "";

    [AliasAs("indtryCtgyCode")]
    public string IndtryCtgyCode { get; set; } = "";

    [AliasAs("customerCode")]
    public string CustomerCode { get; set; } = "";

    [AliasAs("contactor")]
    public string Contactor { get; set; } = "";

    [AliasAs("registerDateStr")]
    public string RegisterDateStr { get; set; } = "";

    [AliasAs("field02")]
    public string Field02 { get; set; } = "";

    [AliasAs("field03")]
    public string Field03 { get; set; } = "";

    [AliasAs("dontCheckSimilar")]
    public string DontCheckSimilar { get; set; } = "true";

    public V3CreateCompanyRequest(InsuredRequest insuredInfo, int areaCode, ulong phone, V3AddressResponse address,
        string syskeyRequestToken)
    {
        PartyName = insuredInfo.FirstName.TrimEnd() + " " + insuredInfo.LastName.TrimStart();;
        RegisterName = insuredInfo.FirstName.TrimEnd() + " " + insuredInfo.LastName.TrimStart();
        RegisterNumber = insuredInfo.Identity;
        AreaCode = areaCode;
        Phone = phone;
        UpdateDate = DateTime.Now.ToString("dd/MM/yyyy");
        StateId = address.StateId;
        Street = address.Street;
        District = address.District;
        City = address.City;
        ZipCode = address.ZipCode;
        Number = insuredInfo.Address.Number;
        Complement = insuredInfo.Address.Complement;
        SyskeyRequestToken = syskeyRequestToken;
    }
}