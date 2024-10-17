using Refit;

namespace Ebao.V2.DPEM.Api.Auth;

public interface IStartApi
{
    [Get("/insurance/")]
    Task<HttpResponseMessage> StartPageAsync();
}