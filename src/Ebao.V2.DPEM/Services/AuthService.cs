using System.Web;
using Ebao.V2.DPEM.Api.Auth;
using Ebao.V2.DPEM.Api.Auth.Requests;
using Ebao.V2.DPEM.Helpers;
using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Models.Contants;
using Ebao.V2.DPEM.Models.ViewModels.Auth.Requests;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Ebao.V2.DPEM.Services;

/// <summary>
/// Responsável pela autenticação no V3.
/// </summary>
public class AuthService : ServiceBase<AuthService>
{
    private readonly IMemoryCache _memoryCache;
    private readonly RequestInfo _requestInfo;
    private readonly IStartApi _startApi;
    private readonly IAuthApi _authApi;

    public AuthService(ILogger<AuthService> logger, IMemoryCache cache, RequestInfo requestInfo, IStartApi startApi,
        IAuthApi authApi, IMemoryCache memoryCache)
        : base(logger, cache, requestInfo)
    {
        _requestInfo = requestInfo;
        _startApi = startApi;
        _authApi = authApi;
        _memoryCache = memoryCache;
    }

    /// <summary>
    /// Efectua o login no V3.
    /// </summary>
    /// <param name="request">Informações de autenticação.</param>
    public async ValueTask LoginAsync(AuthRequest request)
    {
        Logger.LogInformation("Autenticando usuário {Username}", request.Username);

        var (sysKey, loginSessionId, sessionId, lt, service) = await PrepareSysKeyAsync();

        var login = new v3LoginRequest(request.Username, request.Password, lt);

        var response = await _authApi.LoginAsync(loginSessionId, sysKey, service, login);

        var info = new UserInfo(sysKey, sessionId);
        Cache.Set(RequestInfo.Auth, info);

        var parameters = HttpUtility.ParseQueryString(response.Headers.Location.ToString()).Get(0)
            .Split("?");

        var ticket = parameters[0];
        response = await _authApi.CheckAsync(ticket);

        var checkParameters = HttpUtility.ParseQueryString(response.Headers.Location.ToString().Split("?")[1]);

        info.Syskey = checkParameters.Get(Parameters.Syskey)!;

        Logger.LogInformation("Usuário autenticado com sucesso.");
    }

    /// <summary>
    /// Obtém o syskey_request_token da página inicial.
    /// </summary>
    /// <remarks>
    /// TODO: Transformar o retorno em uma classe.
    /// </remarks>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private async Task<(string sysKey, string loginSessionId, string sessionId, string lt, string service)>
        PrepareSysKeyAsync()
    {
        Logger.LogInformation("Preparando sessão para autenticação.");

        var response = await _startApi.StartPageAsync();
        var parameters = HttpUtility.ParseQueryString(response.Headers.Location.ToString().Split("?")[1]);

        var sysKey = parameters.Get(Parameters.Syskey);
        var service = parameters.Get("service");

        if (string.IsNullOrEmpty(sysKey))
            throw new InvalidOperationException("Syskey not found in request.");

        Logger.LogInformation("Syskey obtido com sucesso. Valor {Syskey}", sysKey);

        var sessionId = response.Headers.GetValues("Set-Cookie")
            .FirstOrDefault(w => w.StartsWith(Parameters.Session));

        if (string.IsNullOrEmpty(sessionId))
            throw new InvalidOperationException("Cookie not found in request.");

        Logger.LogInformation("Sessão obtida com sucesso.");

        Logger.LogDebug($"Syskey: {sysKey}. Session: {sessionId}");

        sessionId = sessionId.Split(";")[0].Split("=")[1];

        var (loginSessionId, lt) = await GetLtAsync(sysKey, service);

        return (sysKey, loginSessionId, sessionId, lt, service);
    }


    /// <summary>
    /// Obtém o camp o lt da página login.
    /// </summary>
    /// <param name="pageHtml">HTML da página de login</param>
    /// <returns>Valor do campo lt.</returns>
    /// <exception cref="InvalidOperationException">Caso o lt não seja encontrado na página.</exception>
    private async ValueTask<(string sessionId, string lt)> GetLtAsync(string syskey, string service)
    {
        Logger.LogInformation("Obtendo lt da página de login.");
        var response =
            await _authApi.LoadPageLoginAsync(syskey, service);

        var sessionId = response.Headers.GetValues("Set-Cookie")
            .FirstOrDefault(w => w.StartsWith(Parameters.Session));

        if (string.IsNullOrEmpty(sessionId))
            throw new InvalidOperationException("Cookie not found in request.");

        sessionId = sessionId.Split(";")[0].Split("=")[1];

        var pageHtml = await response.Content.ReadAsStringAsync();

        //xpath do campo lt do formulário
        const string ltXPath = "//*[@name='lt']";

        var reader = new HtmlReader();
        reader.Load(pageHtml);

        var lt = reader.GetValueFromXPath(ltXPath);

        if (string.IsNullOrEmpty(lt))
            throw new InvalidOperationException("lt not found in page/form.");

        Logger.LogDebug($"lt: {lt}");

        return (sessionId, lt);
    }

    public async ValueTask LogoutAsync()
    {
        _memoryCache.Remove(_requestInfo.Auth);
    }
}