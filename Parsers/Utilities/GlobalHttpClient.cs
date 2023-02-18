using FlareSolverrSharp;

public static class GlobalHttpClient
{
    public static HttpClient Client { get; }

    static GlobalHttpClient()
    {
        HttpMessageHandler handler = new ClearanceHandler("http://localhost:8191/") 
        { 
            ProxyUrl = "http://127.0.0.1:8888", 
            MaxTimeout = 120000 
        };
        Client = new HttpClient(handler);
        Client.Timeout = TimeSpan.FromMinutes(2);
        Client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X x.y; rv:42.0) Gecko/20100101 Firefox/42.0");
    }

    public static async Task<Stream> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        var response = await Client.GetAsync(uri, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
