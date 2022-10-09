using Telegram.Bot.Types;

namespace TgBotForSearchWork.src.TelegramBot.Models;

public class User
{
    public long ChatId { get; set; }
    public Dictionary<Uri, Vacancy?> UrisToVacancies { get; set; } = new();    

    public User()
    {    }

    public User(long chatId, IEnumerable<string>? urls = null)
    {
        ChatId = chatId;
        if (urls is not null)
        {
            foreach (var url in urls)
            {
                UrisToVacancies.Add(new Uri(url), null);
            }
        }
    }
}
