public static class GlobalHttpClient
{
    public static HttpClient Client { get; }

    static GlobalHttpClient()
    {
        Client = new HttpClient();
        Client.Timeout = TimeSpan.FromMinutes(2);
    }

    public static async Task<Stream> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        var response = await Client.GetAsync(uri, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
