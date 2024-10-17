using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Models.ViewModels.Auth.Requests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Ebao.V2.DPEM.Services;

namespace Ebao.V2.DPEM.Services;

public class UserService : ServiceBase<UserService>
{
    private readonly AuthService _authService;

    public UserService(ILogger<UserService> logger, IMemoryCache cache, RequestInfo requestInfo, AuthService authService)
        : base(logger, cache, requestInfo)
    {
        _authService = authService;
    }

    public string GetSyskeyRequestToken()
    {
        var user = Cache.Get<UserInfo>(RequestInfo.Auth);

        if (user == null)
            throw new NullReferenceException("User not found.");

        return user.Syskey;
    }

    /// <summary>
    /// Autentica o usuário antes de realizar operações.
    /// </summary>
    public async ValueTask AuthenticateAsync()
    {
        Logger.LogInformation("Autenticando usuário para impressão.");

        var authRequest = new AuthRequest()
        {
            Username = Config.IntegrationConfig.EbaoLogin,
            Password = Config.IntegrationConfig.EbaoPassword
        };

        await _authService.LoginAsync(authRequest);

        Logger.LogInformation("Autenticação concluída com sucesso.");
    }

}
