using MongoDB.Driver;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Utilities;

public interface IMongoDbContext
{
    public IMongoCollection<UriToVacancies> UriToVacanciesCollection { get; }

    public IMongoCollection<User> UserCollection { get; }

    public IMongoCollection<TDocument> GetCollection<TDocument>(string? name = null, MongoCollectionSettings? settings = null);
}
