using Ebao.V2.DPEM.Api.Binding.Requests;
using Refit;

namespace Ebao.V2.DPEM.Api.Binding;

public interface IAuthBindingApi
{
    [Post("/api/auth/Login")]
    [Headers("Content-Type: application/x-www-form-urlencoded")]
    Task<ApiResponse<string>> Login([Body(BodySerializationMethod.UrlEncoded)] LoginRequest request,
        [Header("Ocp-Apim-Subscription-Key")] string ocpApimSubscriptionKey);
}