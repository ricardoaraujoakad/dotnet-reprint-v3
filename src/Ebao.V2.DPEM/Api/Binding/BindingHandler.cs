using System.Net;
using System.Text;

namespace Ebao.V2.DPEM.Api.Binding;

public class BindingHandler : DelegatingHandler
{
    public BindingHandler()
    {
        InnerHandler = new HttpClientHandler
        {
            AllowAutoRedirect = false,
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
        };
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.Content != null)
        {
            var content = await request.Content.ReadAsStringAsync().ConfigureAwait(false);
            request.Content = new StringContent(content, Encoding.UTF8, "application/json");
        }

        return await base.SendAsync(request, cancellationToken);
    }
}