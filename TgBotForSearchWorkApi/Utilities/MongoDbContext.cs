using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Telegram.Bot.Types;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Utilities;

[SingletonService]
public class MongoDbContext
{
    private readonly IMongoDatabase _database;

    public IMongoCollection<UriToVacancies> UriToVacanciesCollection { get => GetCollection<UriToVacancies>("UrisToVacancies"); }

    public MongoDbContext(IMongoDatabase database)
    {
        _database = database;
        var indexKeysDefinition = Builders<UriToVacancies>.IndexKeys.Ascending(uri => uri.HashedUrl).Ascending(uri => uri.ChatId);
        var indexModel = new CreateIndexModel<UriToVacancies>(indexKeysDefinition, new CreateIndexOptions() { Unique = true});
        UriToVacanciesCollection.Indexes.CreateOne(indexModel);
    }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string? name = null, MongoCollectionSettings? settings = null)
    {
        return _database.GetCollection<TDocument>(name ?? typeof(TDocument).Name + 's', settings);
    }
}
