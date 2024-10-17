using System.Net;
using Microsoft.Extensions.Caching.Memory;

namespace Ebao.V2.DPEM.Api.Handlers;

public class AuthHandler : RequestHandler
{
    private object _state;

    public AuthHandler(IServiceProvider provider, IMemoryCache cache) : base(cache, provider)
    {
        InnerHandler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (_state != Config.State)
        {
            _state = Config.State;
            var har = InnerHandler as HttpClientHandler;
            // ClearCookies(har.CookieContainer);
        }

        return base.SendAsync(request, cancellationToken);
    }
    
    
}