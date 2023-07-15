namespace TgBotForSearchWorkApi.Utilities;

using System.Text;
using Parsers.Constants;

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
