using MongoDB.Driver;
using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.Utilities;

internal static class MongoDb
{
    private static MongoClient _mongoClient;
    private static IMongoDatabase _database;

    public static IMongoCollection<User> UserCollection { get => GetCollection<User>(); }

    static MongoDb()
    {
        _mongoClient = new("mongodb://localhost:27017");
        _database = _mongoClient.GetDatabase("TgBotForSearchWorkDb");
    }

    public static IMongoCollection<TDocument> GetCollection<TDocument>(MongoCollectionSettings? settings = null)
    {
        return _database.GetCollection<TDocument>(typeof(TDocument).Name +'s', settings);
    }
}
