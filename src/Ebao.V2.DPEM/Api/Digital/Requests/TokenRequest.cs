using Refit;

namespace Ebao.V2.DPEM.Api.Digital.Requests;

public class TokenRequest
{
    [AliasAs("username")]
    public string Username { get; set; }

    [AliasAs("password")]
    public string Password { get; set; }

    [AliasAs("grant_type")]
    public string GrantType { get; set; } = "password";

    [AliasAs("client_id")]
    public string ClientId { get; set; } = "portal_argo";

    [AliasAs("client_secret")]
    public string ClientSecret { get; set; } = "portal_argo_secret";
}