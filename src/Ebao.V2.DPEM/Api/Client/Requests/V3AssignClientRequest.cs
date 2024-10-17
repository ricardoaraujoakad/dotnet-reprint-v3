using Ebao.V2.DPEM.Api.Client.Responses;
using Refit;

namespace Ebao.V2.DPEM.Api.Client.Requests;

public record V3AssignClientRequest
{
    [AliasAs("orgInsured")]
    public long? OrgInsured { get; set; }

    [AliasAs("indInsured")]
    public long? IndInsured { get; }

    [AliasAs("custradio")]
    public long? Custradio => IndInsured ?? OrgInsured;

    [AliasAs("fromUrl")]
    public string FromUrl => "API.gs.pol.nb.action.NewBizAction?operId=LoaderCustInfo";

    [AliasAs("indPartyIdDis")]
    public long? IndPartyIdDis => IndInsured;

    [AliasAs("orgPartyIdLis")]
    public long? OrgPartyIdDis => OrgInsured;

    [AliasAs("indPayer")]
    public long? IndPayer => IndInsured;

    [AliasAs("isCustomer")]
    public string? IsCustomer => OrgInsured != null ? "1" : null; 

    [AliasAs("orgPayer")]
    public long? OrgPayer => OrgInsured;

    [AliasAs("newPartyId")]
    public long? NewPartyId => IndInsured ?? OrgInsured;

    [AliasAs("operId")]
    public string OperId => "SaverDECustInfo";

    [AliasAs("syskey_request_token")]
    public string SyskeyRequestToken { get; }

    [AliasAs("address01")]
    public string City { get; }

    [AliasAs("address02")]
    public string District { get; }

    [AliasAs("address03")]
    public int StreetTypeId => 86;

    [AliasAs("address03firstLoad")]
    public bool Address03FirstLoad => false;

    [AliasAs("address04")]
    public string Street { get; }

    [AliasAs("address07")]
    public string ZipCode { get; }

    [AliasAs("regionLv1")]
    public int CountryId => 130;

    [AliasAs("regionLv1firstLoad")]
    public bool RegionLv1FirstLoad => false;

    [AliasAs("regionLv2")]
    public int StateId { get; }

    [AliasAs("regionLv2firstLoad")]
    public bool RegionLv2FirstLoad => false;

    [AliasAs("submissionCustId")]
    public int SubmissionCustId => 0;

    [AliasAs("motorLevela")]
    public int MotorLevela => 0;

    [AliasAs("nonmotorLevelb")]
    public string NonMotorLevelb => "0";

    [AliasAs("mpContractId")]
    public string MpContractId => "null";

    [AliasAs("address05")]
    public string Number { get; set; }

    [AliasAs("address06")]
    public string Complement { get; set; }

    public V3AssignClientRequest(V3ClientResponse client, string syskeyRequestToken)
    {
        var address = client.Address;

        if (client.Identity.Length == 11)
            IndInsured = client.Id;
        else
            OrgInsured = client.Id;

        StateId = address.StateId;
        City = address.City;
        District = address.District;
        Street = address.Street;
        ZipCode = address.ZipCode;
        Complement = address.Complement;
        Number = address.Number;

        SyskeyRequestToken = syskeyRequestToken;
    }
}