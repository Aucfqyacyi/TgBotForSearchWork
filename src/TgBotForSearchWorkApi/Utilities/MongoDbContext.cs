using AutoDIInjector.Attributes;
using MongoDB.Driver;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Utilities;

[SingletonService(typeof(IMongoDbContext))]
public class MongoDbContext : IMongoDbContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<UriToVacancies> UriToVacanciesCollection { get => GetCollection<UriToVacancies>("UrisToVacancies"); }

    public IMongoCollection<User> UserCollection { get => GetCollection<User>(); }

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
        var indexKeysDefinition = Builders<UriToVacancies>.IndexKeys.Ascending(uri => uri.HashedUrl).Ascending(uri => uri.ChatId);
        var indexModel = new CreateIndexModel<UriToVacancies>(indexKeysDefinition, new CreateIndexOptions() { Unique = true });
        UriToVacanciesCollection.Indexes.CreateOne(indexModel);
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string? name = null, MongoCollectionSettings? settings = null)
    {
        return _database.GetCollection<TDocument>(name ?? typeof(TDocument).Name + 's', settings);
    }
}
