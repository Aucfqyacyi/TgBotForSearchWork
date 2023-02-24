namespace Parsers.Models;

public class Vacancy
{
    public ulong Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public Vacancy(ulong id, string title, string url, string shortDescription)
    {
        Id = id;
        Title = title;
        Url = url;
        ShortDescription = shortDescription;
    }

    public string Present()
    {
        return $"[{Title}]({Url})\n{ShortDescription}";
    }
}
