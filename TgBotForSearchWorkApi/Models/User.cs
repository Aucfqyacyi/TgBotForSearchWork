using MongoDB.Bson.Serialization.Attributes;

namespace TgBotForSearchWork.Models;

public partial class User
{
    [BsonId] public long ChatId { get; set; }
    [BsonElement(nameof(Urls))] public List<UrlToVacancies> _urls = new();
}
