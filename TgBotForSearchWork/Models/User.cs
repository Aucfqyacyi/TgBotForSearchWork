using MongoDB.Bson.Serialization.Attributes;

namespace TgBotForSearchWork.Models;

public class User
{
    private List<UrlToVacancies> _urls = new();

    [BsonId]
    public long ChatId { get; set; }
    [BsonElement] 
    public IReadOnlyList<UrlToVacancies> Urls { get => _urls; set => _urls.AddRange(value); }

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
}
