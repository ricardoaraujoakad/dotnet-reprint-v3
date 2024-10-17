using Refit;

namespace Ebao.V2.DPEM.Api.Binding.Requests;

public class LoginRequest
{
    [AliasAs("email")]
    public string Email { get; set; }

    [AliasAs("password")]
    public string Password { get; set; }
}