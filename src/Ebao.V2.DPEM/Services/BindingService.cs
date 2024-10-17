using Ebao.V2.DPEM.Api.Binding;
using Ebao.V2.DPEM.Api.Binding.Requests;
using Ebao.V2.DPEM.Api.Binding.Responses;
using Ebao.V2.DPEM.Helpers.Extensions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace Ebao.V2.DPEM.Services;

public class BindingService
{
    private readonly IMemoryCache _cache;
    private readonly IAuthBindingApi _authBindingApi;
    private readonly IProposalApi _proposalApi;

    public BindingService(IMemoryCache cache, IProposalApi proposalApi, IAuthBindingApi authBindingApi)
    {
        _cache = cache;
        _proposalApi = proposalApi;
        _authBindingApi = authBindingApi;
    }

    private async ValueTask<string> AuthAsync()
    {
        if (_cache.TryGetValue("BindingToken", out string token))
        {
            return token;
        }

        var response = await _authBindingApi.Login(new LoginRequest
        {
            Email = Config.IntegrationConfig.BindingLogin,
            Password = Config.IntegrationConfig.BindingPassword
        }, Config.IntegrationConfig.BindingSubscriptionKey);

        if (response.IsSuccessStatusCode)
        {
            token = "Bearer " + response.Content;
            token = token.Replace("\"", "");
            _cache.Set("BindingToken", token, TimeSpan.FromMinutes(30));
            return token;
        }

        throw new Exception("Failed to authenticate with Binding API");
    }


    public async Task UpdateProposalStatusAsync(BatchResponse batchResponse, TimeSpan runningTime, string? PolicyNumber,
        DateTime? bookedAt, Exception? ex)
    {
        var logText = "";
        var status = PolicyNumber.IsNullOrEmpty() ? 2 : 5;

        if (ex != null)
            logText = ex.Message;

        var updateRequest = new UpdateProposalStatusRequest
        {
            MessageId = batchResponse.Id,
            ProposalId = batchResponse.ProposalId,
            Status = status,
            PolicyNumber = PolicyNumber,
            BookedAt = DateTime.Now.ConvertToTimeZone(),
            RunningTime = runningTime,
            BookingLogs = new List<BookingLog>
            {
                new()
                {
                    LogText = logText,
                    LogType = 1,
                    LogDate = DateTime.Now.ConvertToTimeZone()
                }
            }
        };

        var response = await _proposalApi.UpdateProposalStatusAsync(await AuthAsync(), updateRequest,
            Config.IntegrationConfig.BindingSubscriptionKey);

        if (response.IsSuccessStatusCode)
            Console.WriteLine("Proposal status updated successfully!");
        else
            Console.WriteLine($"Failed to update proposal status: {response.Error.Content}");
    }

    public async ValueTask<IReadOnlyList<BatchResponse>?> GetNextProposalAsync()
    {
        var token = await AuthAsync();

        return await _proposalApi.GetLotsAsync(token, Config.IntegrationConfig.BindingSubscriptionKey);
    }
}