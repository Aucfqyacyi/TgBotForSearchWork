using MongoDB.Bson.Serialization.Attributes;
using Telegram.Bot.Types;

namespace TgBotForSearchWorkApi.Models;

public partial class User
{
    [BsonIgnore] public IReadOnlyList<UrlToVacancies> Urls { get => _urls; }

    public User()
    { }

    public User(long chatId, IEnumerable<string>? urls = null)
    {
        ChatId = chatId;
        if (urls is not null)
        {
            foreach (var url in urls)
            {
                _urls.Add(new UrlToVacancies(url));
            }
        }
    }

    public void AddUrlToVacancias(UrlToVacancies? urlToVacancies)
    {
        if (urlToVacancies is null)
            return;

        _urls.Add(urlToVacancies);
    }

    public void RemoveUrlToVacancias(UrlToVacancies? urlToVacancies)
    {
        if (urlToVacancies is null)
            return;

        _urls.Remove(urlToVacancies);
    }

    public void RemoveUrlToVacancias(int index)
    {
        if (index == default)
            return;

        _urls.RemoveAt(index);
    }

    public void ActivateUrlToVacancias(int index)
    {
        if (index == default)
            return;

        Urls[index].IsActivate = true;
    }
}
