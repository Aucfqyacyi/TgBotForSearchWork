namespace TgBotForSearchWork.src.Other;

internal static class GHttpClient
{
	public static HttpClient Client { get; }

	static GHttpClient()
	{
		Client = new HttpClient();
	}
}
