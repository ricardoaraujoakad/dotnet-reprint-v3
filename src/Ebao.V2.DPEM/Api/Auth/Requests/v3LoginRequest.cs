using Refit;

namespace Ebao.V2.DPEM.Api.Auth.Requests;

public record v3LoginRequest(string Username, string Password, string Lt)
{
    [AliasAs("_eventId")]
    public string EventId { get; set; } = "submit";

    [AliasAs("submit")]
    public string Submit { get; set; } = "LOGIN";

    [AliasAs("username")]
    public string Username { get; set; } = Username;

    [AliasAs("password")]
    public string Password { get; set; } = Password;

    [AliasAs("lt")]
    public string Lt { get; set; } = Lt;
}