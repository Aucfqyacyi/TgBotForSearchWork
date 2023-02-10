using MongoDB.Driver;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Utilities;

[SingletonService]
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<User> UserCollection { get => GetCollection<User>(); }

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>(MongoCollectionSettings? settings = null)
    {
        return _database.GetCollection<TDocument>(typeof(TDocument).Name + 's', settings);
    }
}
