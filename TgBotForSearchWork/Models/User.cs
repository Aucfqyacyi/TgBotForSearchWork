using MongoDB.Bson.Serialization.Attributes;

namespace TgBotForSearchWork.Models;

public partial class User
{
    [BsonId] public long ChatId { get; set; }
    [BsonElement(nameof(Urls))] private List<UrlToVacancies> _urls = new();
    [BsonElement] public string LastCommand { get; set; } = string.Empty;
}
