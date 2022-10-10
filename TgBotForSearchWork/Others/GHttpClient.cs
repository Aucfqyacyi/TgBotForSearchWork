namespace TgBotForSearchWork.Others;

internal static class GHttpClient
{
	public static HttpClient Client { get; }

	static GHttpClient()
	{
		Client = new HttpClient();
	}

    public static async Task<Stream> GetAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        return await GetAsync(uri.OriginalString, cancellationToken);
    }

    public static async Task<Stream> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await Client.GetAsync(url, cancellationToken);
        return await response.Content.ReadAsStreamAsync(cancellationToken);
    }
}
