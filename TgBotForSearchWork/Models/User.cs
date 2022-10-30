namespace TgBotForSearchWork.Models;

public class User
{
    public long ChatId { get; set; }
    public Dictionary<Url, Vacancy?> UrlsToVacancies { get; set; } = new();

    public User()
    { }

    public User(long chatId, IEnumerable<string>? urls = null)
    {
        ChatId = chatId;
        if (urls is not null)
        {
            foreach (var url in urls)
            {
                UrlsToVacancies.Add(new Url(url), null);
            }
        }
    }
}
