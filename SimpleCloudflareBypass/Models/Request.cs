using System.ComponentModel.DataAnnotations;

namespace SimpleCloudflareBypass.Models;

public record Request
{
    public string Url { get; set; } = null!;
    public string? IdOnLoadedPage { get; set; }
    public int Timeout { get; set; } = 30;
}
