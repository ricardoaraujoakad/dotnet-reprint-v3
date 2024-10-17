using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using Ebao.V2.DPEM.Exceptions;
using Ebao.V2.DPEM.Models;
using Ebao.V2.DPEM.Models.Contants;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Core;

namespace Ebao.V2.DPEM.Api.Handlers;

public class RequestHandler : DelegatingHandler
{
    private RequestInfo RequestInfo => _provider.GetRequiredService<RequestInfo>();
    private readonly IServiceProvider _provider;
    private readonly IMemoryCache _cache;

    public RequestHandler(IMemoryCache cache, IServiceProvider provider)
    {
        _cache = cache;
        _provider = provider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
        request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko");

        var innerHandle = InnerHandler as HttpClientHandler;
        if (_cache.TryGetValue<UserInfo>(RequestInfo.Auth, out var userInfo))
            LoadParameters(request, userInfo);
        else
            ClearCookies(innerHandle.CookieContainer);

        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        //Nas requests do eBao, é comum sempre ter o comentário iniciando e terminando de uma região de Exception, então contamos quantas vezes a palavra aparece.
        //Eg:
        //<!--begin of AppException handling-->
        // 
        // <!--end of AppException handling-->
        //Requests normais, aparecem apenas 2 vezes a palavra exception. Se aparecerem mais do que isso, é o stacktrace do erro e consequentemente a request falhou.
        //Obs: Mesmo dando erro a request retorna 200!
        var exceptionCount = content.Split("Exception").Length - 1;

        //Uma outra forma de validar, é verificando se a palavra Error! está no conteúdo da request.
        //Uma validação não é redudante a outra, as duas normalmente ocorrem juntas, mas podem ocorrer uma ou outra separadamente.
        if (content.Contains("Error!") || exceptionCount > 2)
        {
            Log.Information("eBao HTTP Response: {Response}",
                content.Trim().Replace(Environment.NewLine, string.Empty));

            if (request.Content != null)
            {
                var rawContent = await request.Content.ReadAsStringAsync(cancellationToken);
                Log.Information("eBao HTTP Request: {Request}",
                    rawContent.Trim().Replace(Environment.NewLine, string.Empty));
            }

            FilterException(content);

            throw new HttpRequestException("Falha ao executar request.");
        }

        return response;
    }

    static void ClearCookies(CookieContainer cookieContainer)
    {
        var table = cookieContainer.GetType().GetField("m_domainTable",
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        if (table != null)
        {
            var domains = (IDictionary)table.GetValue(cookieContainer)!;
            if (domains != null)
            {
                domains.Clear();
            }
        }
    }

    private static void LoadParameters(HttpRequestMessage request, UserInfo userInfo)
    {
        request.Headers.Remove("Cookie");

        AddSession(request, userInfo);
        AddSyskey(request, userInfo);
    }

    private static void AddSession(HttpRequestMessage request, UserInfo userInfo)
    {
        request.Headers.Add("Cookie", $"JSESSIONID={userInfo.SessionId}");
    }

    private static void FilterException(string content)
    {
        if (!content.Contains("We already have the policy")) return;

        var unknownDuplicateException = new InvalidOperationException(
            "Já existe uma apólice emitda para este certificado (Apólice não encontrada).");

        const string pattern = @"Validation Check Result: \(We already have the policy &lt;(\d+)&gt;";

        var match = Regex.Match(content, pattern);

        if (!match.Success) throw unknownDuplicateException;

        var policyNumber = match.Groups[1].Value;

        if (policyNumber.StartsWith("027"))
            throw new DuplicateCertificateException(policyNumber);

        throw unknownDuplicateException;
    }

    private static void AddSyskey(HttpRequestMessage request, UserInfo userInfo)
    {
        var builder = new UriBuilder(request.RequestUri!);
        var query = HttpUtility.ParseQueryString(builder.Query);

        if (query.AllKeys.Any(w => w == Parameters.Syskey))
            query[Parameters.Syskey] = userInfo.Syskey;

        builder.Query = query.ToString();
        request.RequestUri = builder.Uri;
    }
}