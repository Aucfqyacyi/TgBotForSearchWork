using MongoDB.Bson.Serialization.Attributes;
using Telegram.Bot.Types;

namespace TgBotForSearchWorkApi.Models;

public partial class User
{
    public User()
    { }

    public User(long chatId)
    {
        Id = chatId;
    }
}
