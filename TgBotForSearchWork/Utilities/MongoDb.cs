using MongoDB.Driver;

namespace TgBotForSearchWork.Utilities;

internal static class MongoDb
{
    private static MongoClient _mongoClient;
    private static IMongoDatabase _database;

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
