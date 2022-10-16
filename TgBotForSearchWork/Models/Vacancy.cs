namespace TgBotForSearchWork.Models;

public class Vacancy
{
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public Vacancy(string title, string url, string shortDescription)
    {
        Title = title;
        Url = url;
        ShortDescription = shortDescription;
    }

    public string Present()
    {
        return $"[{Title}]({Url}){ShortDescription}";
    }
}
