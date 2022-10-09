using Telegram.Bot.Types;
using TgBotForSearchWork.src.Extensions;

namespace TgBotForSearchWork.src.TelegramBot.Models;

public class User
{
    public long ChatId { get; set; }
    public Dictionary<string, Uri> HashsToUris { get; set; } = new();
    public Vacancy? LastVacancy { get; set; }
    public User()
    {    }

    public User(long chatId, IEnumerable<string>? urls = null)
    {
        ChatId = chatId;
        if (urls is not null)
        {
            foreach (var url in urls)
            {
                HashsToUris.Add(url.GetMD5(), new Uri(url));
            }
        }
    }
}
