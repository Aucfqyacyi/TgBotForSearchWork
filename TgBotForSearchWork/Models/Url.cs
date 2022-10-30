namespace TgBotForSearchWork.Models;

public class Url
{
    public Uri Uri { get; set; }
    public bool IsOff { get; set; }
    public string OriginalString { get => Uri.OriginalString; }
    public string Host { get => Uri.Host; }
    public Url(Uri uri, bool isBuilded = false)
    {
        Uri = uri;
        IsOff = isBuilded;
    }

    public Url(string url, bool isBuilded = false) : this(new Uri(url), isBuilded)
    {
    }

    public static implicit operator Uri(Url url)
    {
        return url.Uri;
    }
}
