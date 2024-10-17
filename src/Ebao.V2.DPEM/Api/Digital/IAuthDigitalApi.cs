using Ebao.V2.DPEM.Api.Digital.Requests;
using Ebao.V2.DPEM.Api.Digital.Responses;
using Refit;

namespace Ebao.V2.DPEM.Api.Digital;

public interface IAuthDigitalApi
{
    [Post("/security/connect/token")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    Task<ApiResponse<TokenResponse>> GetTokenAsync([Body(BodySerializationMethod.UrlEncoded)] TokenRequest request,
        [Header("Ocp-Apim-Subscription-Key")] string ocpApimSubscriptionKey);
}