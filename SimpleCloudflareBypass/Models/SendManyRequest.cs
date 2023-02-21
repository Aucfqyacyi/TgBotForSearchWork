namespace SimpleCloudflareBypass.Models;

public class SendManyRequest
{
    public List<SendRequest> Requests { get; set; } = new();
}
