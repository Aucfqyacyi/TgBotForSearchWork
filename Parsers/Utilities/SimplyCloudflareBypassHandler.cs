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
            _simpleCloudflareBypassUrl += "send";
        else
            _simpleCloudflareBypassUrl += "/send";       
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage httpRequestMessage, CancellationToken cancellationToken)
    {
        SiteType siteType = SiteTypesToUris.HostsToSiteTypes[httpRequestMessage.RequestUri!.Host];
        if (siteType != SiteType.Dou)
            return base.SendAsync(httpRequestMessage, cancellationToken);
        string request = $$"""
                    {
                        "Url": "{{httpRequestMessage.RequestUri.OriginalString}}", 
                        "IdOnLoadedPage": "txtGlobalSearch"
                    }
                    """;
        httpRequestMessage.RequestUri = new Uri(_simpleCloudflareBypassUrl);
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.Content = new StringContent(request, Encoding.UTF8, "application/json");
        return base.SendAsync(httpRequestMessage, cancellationToken);
    }
}
