
using AngleSharp.Dom;
using System.Net;

public static class GlobalHttpClient
{
    public static HttpClient Client { get; }

    static GlobalHttpClient()
    {
        Client = new HttpClient();
        Client.Timeout = TimeSpan.FromMinutes(2);
        Client.DefaultRequestHeaders.Add("User-Agent", "TgBotForSearchWork");
    }

    public static async Task<Stream> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        var response = await Client.GetAsync(uri, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
