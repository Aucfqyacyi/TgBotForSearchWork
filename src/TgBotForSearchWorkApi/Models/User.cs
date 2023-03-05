using MongoDB.Bson.Serialization.Attributes;

namespace TgBotForSearchWorkApi.Models;

public class User
{
    [BsonId] public long ChatId { get; set; }
    [BsonElement] public bool IsActivated { get; set; }
    [BsonElement] public int DescriptionLength { get; set; }

    public User(long chatId, int descriptionLength = 1000, bool isActivated = true)
    {
        ChatId = chatId;
        DescriptionLength = descriptionLength;
        IsActivated = isActivated;
    }
}
