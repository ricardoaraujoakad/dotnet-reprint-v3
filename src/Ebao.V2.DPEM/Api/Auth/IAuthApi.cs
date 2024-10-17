using Ebao.V2.DPEM.Api.Auth.Requests;
using Ebao.V2.DPEM.Models.Contants;
using Refit;

namespace Ebao.V2.DPEM.Api.Auth;

public interface IAuthApi
{
    [Get("/insurance/mainMenu.do")]
    Task<HttpResponseMessage> MainMenuAsync();


    [Post("/cas/login;jsessionid={sessionId}")]
    Task<HttpResponseMessage> LoginAsync(
        string sessionId,
        [Query] [AliasAs(Parameters.Syskey)] string sysKey,
        [Query] [AliasAs("service")] string service,
        [Body(BodySerializationMethod.UrlEncoded)] v3LoginRequest loginRequest);

    [Get("/cas/login")]
    Task<HttpResponseMessage> LoadPageLoginAsync([Query] [AliasAs(Parameters.Syskey)] string sysKey,
        [Query] string service);

    [Get("/insurance/j_acegi_cas_security_check.do")]
    Task<HttpResponseMessage> CheckAsync([Query] [AliasAs("ticket")] string ticket);
}