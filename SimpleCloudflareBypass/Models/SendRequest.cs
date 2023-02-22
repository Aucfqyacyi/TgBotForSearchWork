using System.ComponentModel.DataAnnotations;

namespace SimpleCloudflareBypass.Models;

public class SendRequest
{
    public string Url { get; set; } = null!;
    public string? IdOnLoadedPage { get; set; }
    public int Timeout { get; set; }

    public SendRequest(string url, string? idOnLoadedPage = null, int timeout = 120)
    {
        Url = url;
        IdOnLoadedPage = idOnLoadedPage;
        Timeout = timeout;
    }
}
