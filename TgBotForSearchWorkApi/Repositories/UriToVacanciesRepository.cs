using MongoDB.Bson;
using MongoDB.Driver;
using Parsers.Constants;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Repositories;

[SingletonService]
public class UriToVacanciesRepository
{
    private readonly MongoDbContext _mongoContext;

    public UriToVacanciesRepository(MongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    private FilterDefinition<UriToVacancies> GetFilterById(ObjectId urlId)
    {
        return Builders<UriToVacancies>.Filter.Eq(url => url.Id, urlId);
    }

    public void InsertMany(IReadOnlyList<UriToVacancies> uriToVacancies, CancellationToken cancellationToken)
    {
        _mongoContext.UriToVacanciesCollection.InsertMany(uriToVacancies, null, cancellationToken);  
    }

    public void InsertOne(UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        _mongoContext.UriToVacanciesCollection.InsertOne(uriToVacancies, null, cancellationToken);
    }

    public List<UriToVacancies> GetAll(long chatId, SiteType siteType, CancellationToken cancellationToken)
    {
        var chatIdFilter = Builders<UriToVacancies>.Filter.Eq(url => url.ChatId, chatId);
        var siteTypeFilter = Builders<UriToVacancies>.Filter.Eq(url => url.SiteType, siteType);
        return _mongoContext.UriToVacanciesCollection.FindSync(chatIdFilter & siteTypeFilter, null, cancellationToken)
                                                     .ToList();
    }

    public UriToVacancies Get(ObjectId urlId, CancellationToken cancellationToken)
    {
        UriToVacancies? uriToVacancies = GetOrDefault(urlId, cancellationToken);
        if (uriToVacancies is null)
            throw new Exception($"Url with id({urlId}) doesnot exist.");
        return uriToVacancies;
    }

    public UriToVacancies? GetOrDefault(ObjectId urlId, CancellationToken cancellationToken)
    {
        return _mongoContext.UriToVacanciesCollection.FindSync(GetFilterById(urlId), null, cancellationToken)
                                                     .FirstOrDefault(cancellationToken);
    }

    public void Replace(UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        ReplaceOptions? replaceOptions = null;
        _mongoContext.UriToVacanciesCollection.ReplaceOne(GetFilterById(uriToVacancies.Id), uriToVacancies, replaceOptions, cancellationToken);
    }

    public void Delete(ObjectId urlId, CancellationToken cancellationToken)
    {
        _mongoContext.UriToVacanciesCollection.DeleteOne(GetFilterById(urlId), cancellationToken);
    }

    public void Activate(ObjectId urlId, bool isActivated, CancellationToken cancellationToken)
    {
        var update = Builders<UriToVacancies>.Update.Set(url => url.IsActivated, isActivated);
        _mongoContext.UriToVacanciesCollection.UpdateOne(GetFilterById(urlId), update, null, cancellationToken);
    }

    public bool IsActivated(ObjectId urlId)
    {
        return _mongoContext.UriToVacanciesCollection.Find(GetFilterById(urlId)).Project(uri => uri.IsActivated).FirstOrDefault();
    }
}
