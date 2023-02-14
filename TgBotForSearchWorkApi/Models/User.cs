using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TgBotForSearchWorkApi.Models;

public partial class User
{
    [BsonId] public long Id { get; set; }

    [BsonElement] public List<ObjectId> UrlIds { get; set; } = new();
}
