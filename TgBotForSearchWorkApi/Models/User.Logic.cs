using MongoDB.Bson.Serialization.Attributes;

namespace TgBotForSearchWork.Models;

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
        if (urlToVacancies == null)
            return;

        _urls.Add(urlToVacancies);
    }
}
