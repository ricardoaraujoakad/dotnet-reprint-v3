using Ebao.V2.DPEM.Api.Binding.Requests;
using Ebao.V2.DPEM.Api.Binding.Responses;
using Refit;

namespace Ebao.V2.DPEM.Api.Binding;

public interface IProposalApi
{
    [Get("/api/Proposal/GetLots/")]
    Task<IReadOnlyList<BatchResponse>> GetLotsAsync([Header("Authorization")] string authorization,
        [Header("Ocp-Apim-Subscription-Key")] string ocpApimSubscriptionKey);

    [Post("/api/Proposal/UpdateStatus")]
    Task<ApiResponse<string>> UpdateProposalStatusAsync(
        [Header("Authorization")] string authorization,
        [Body] UpdateProposalStatusRequest request,
        [Header("Ocp-Apim-Subscription-Key")] string ocpApimSubscriptionKey);
}