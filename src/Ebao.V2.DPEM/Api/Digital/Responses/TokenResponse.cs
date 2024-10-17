using System.Text.Json.Serialization;
using Refit;

namespace Ebao.V2.DPEM.Api.Digital.Responses;

public class TokenResponse
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("authentication_methods")]
    public string[] AuthenticationMethods { get; set; }

    [JsonPropertyName("should_pass_2fa")]
    public bool ShouldPass2Fa { get; set; }
}