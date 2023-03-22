namespace Parsers.Models;

public class Vacancy 
{
    public ulong Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }

    public Vacancy(ulong id, string title, string url, string description)
    {
        Id = id;
        Title = title;
        Url = url;
        Description = description;
    }

    public string Present()
    {
        return $"<a href=\"{Url}\">{Title}</a>\n{Description}";
    }
}
