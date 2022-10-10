namespace TgBotForSearchWork.VacancyParsers.Models;

public class Vacancy
{
    public string Title { get; set; } = string.Empty;
    public string ShortDescription { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public string Present()
    {
        return $"[{Title}]({Url})\n{ShortDescription}\nPublication date - {Date}";
    }
}
