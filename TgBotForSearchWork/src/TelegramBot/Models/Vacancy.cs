namespace TgBotForSearchWork.src.TelegramBot.Models;

public class Vacancy
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;

    public string Present()
    {
        return $"[{Title}]({Url})\n{Description}\nPublication date - {Date}";
    }
}
