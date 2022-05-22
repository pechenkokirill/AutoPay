using System.Net.Http;
using System.Net.Http.Headers;

namespace AutoPay.UI.Common.Api;

public class JwtHttpClientHandler : HttpClientHandler
{
    private readonly ConnectionContext _context;

    public JwtHttpClientHandler(ConnectionContext context)
    {
        _context = context;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        PrepareRequest(request);
        return base.SendAsync(request, cancellationToken);
    }

    private void PrepareRequest(HttpRequestMessage request)
    {
        if (!string.IsNullOrEmpty(_context.Token))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _context.Token);
        }
    }
}