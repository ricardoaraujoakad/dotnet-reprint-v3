using Refit;

namespace Ebao.V2.DPEM.Api.Client.Requests;

public record V3CompanyQueryRequest(string syskey, string identity)
{
    [AliasAs("method")]
    public string Method => "searchOrg";

    [AliasAs("do")] 
    public int Do => 1;

    [AliasAs("roleType")] 
    public int Role { get; set; } = 44;

    [AliasAs("ptyCategory")] 
    public int Category { get; set; } = 1;

    [AliasAs("organizationIdNumber")] 
    public string Identity { get; set; } = identity;

    [AliasAs("organizationIdType")]
    public int IdentityType => 1;

    [AliasAs("syskey_request_token")]
    public string Syskey => syskey;
}