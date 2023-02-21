using Parsers.Constants;
using SimpleCloudflareBypass.Models;
using System.Net.Http.Json;

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
        SendRequest request = new(httpRequestMessage.RequestUri.OriginalString, "container");
        httpRequestMessage.RequestUri = new Uri(_simpleCloudflareBypassUrl);
        httpRequestMessage.Method = HttpMethod.Post;
        httpRequestMessage.Content = JsonContent.Create(request);
        return base.SendAsync(httpRequestMessage, cancellationToken);
    }
}
