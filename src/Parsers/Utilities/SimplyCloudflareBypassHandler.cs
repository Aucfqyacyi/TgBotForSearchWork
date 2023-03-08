using Parsers.Constants;
using System.Text;

namespace Parsers.Utilities;

internal class SimplyCloudflareBypassHandler : DelegatingHandler
{
    private readonly string _simpleCloudflareBypassUrl;

    public SimplyCloudflareBypassHandler(string simpleCloudflareBypassUrl) : base(new HttpClientHandler())
    {
        _simpleCloudflareBypassUrl = simpleCloudflareBypassUrl;
        if (_simpleCloudflareBypassUrl.EndsWith("/"))
            _simpleCloudflareBypassUrl += "getHtml";
        else
            _simpleCloudflareBypassUrl += "/getHtml";
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
        SiteType siteType = SiteTypesToUris.HostsToSiteTypes[httpRequestMessage.RequestUri!.Host];
        if (siteType != SiteType.Dou || httpRequestMessage.RequestUri.OriginalString.Contains("/feeds/"))
            return await base.SendAsync(httpRequestMessage, cancellationToken);
        var response = await base.SendAsync(httpRequestMessage, cancellationToken);
        if (response.IsSuccessStatusCode is false)
            return response;   
        string request = $$"""
                    {
                        "Url": "{{httpRequestMessage.RequestUri.OriginalString}}"
                    }
                    """;
        httpRequestMessage.RequestUri = new Uri(_simpleCloudflareBypassUrl);
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.Content = new StringContent(request, Encoding.UTF8, "application/json");
        return await base.SendAsync(httpRequestMessage, cancellationToken);
    }
}
