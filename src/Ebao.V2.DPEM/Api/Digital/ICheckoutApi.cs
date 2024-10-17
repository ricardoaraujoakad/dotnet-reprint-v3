using Ebao.V2.DPEM.Api.Digital.Requests;
using Refit;

namespace Ebao.V2.DPEM.Api.Digital;

public interface ICheckoutApi
{
    [Put("/checkout/api/checkout/ResponseEbaoPolicyNumber/")]
    [Headers("Content-Type: application/json", "Accept: application/json")]
    Task<ApiResponse<string>> UpdateEbaoPolicyNumberAsync(
        [Header("Authorization")] string authorization,
        [Header("Ocp-Apim-Subscription-Key")] string subscriptionKey,
        [Body] UpdateEbaoPolicyNumberRequest request);
}