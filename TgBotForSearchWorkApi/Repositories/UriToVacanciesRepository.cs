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

    public void InsertMany(IReadOnlyList<UriToVacancies> uriToVacancies, CancellationToken cancellationToken)
    {
        _mongoContext.UriToVacanciesCollection.InsertMany(uriToVacancies, null, cancellationToken);  
    }

    public void InsertOne(UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        _mongoContext.UriToVacanciesCollection.InsertOne(uriToVacancies, null, cancellationToken);
    }

    public List<UriToVacancies> GetAll(long userId, SiteType siteType, CancellationToken cancellationToken)
    {
        var userIdsFilter = Builders<UriToVacancies>.Filter.AnyEq(url => url.UserIds, userId);
        var siteTypeFilter = Builders<UriToVacancies>.Filter.Eq(url => url.SiteType, siteType);
        return _mongoContext.UriToVacanciesCollection.FindSync(userIdsFilter & siteTypeFilter, null, cancellationToken)
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
        return _mongoContext.UriToVacanciesCollection.FindSync(url => url.Id == urlId, null, cancellationToken)
                                                     .FirstOrDefault(cancellationToken);
    }

    public void Replace(UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        ReplaceOptions? replaceOptions = null;
        _mongoContext.UriToVacanciesCollection.ReplaceOne(u => u.Id == uriToVacancies.Id, uriToVacancies, replaceOptions, cancellationToken);
    }

    public UriToVacancies? GetOrDefault(string hashedUrl, CancellationToken cancellationToken)
    {
        return _mongoContext.UriToVacanciesCollection.FindSync(url => url.HashedUrl == hashedUrl, null, cancellationToken)
                                                                    .FirstOrDefault(cancellationToken);
    }

    public UriToVacancies InsertOrUpdate(long userId, UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        UriToVacancies? oldUrl = GetOrDefault(uriToVacancies.HashedUrl, cancellationToken);
        if (oldUrl is null)
        {
            InsertOne(uriToVacancies, cancellationToken);
            return uriToVacancies;
        }
        else
        {
            AddUserId(oldUrl.Id, userId, cancellationToken);
            return oldUrl;
        }
    }

    public void AddUserId(ObjectId urlId, long userId, CancellationToken cancellationToken)
    {
        var update = Builders<UriToVacancies>.Update.AddToSet(url => url.UserIds, userId);
        _mongoContext.UriToVacanciesCollection.UpdateOne(url => url.Id == urlId, update, null, cancellationToken);
    }

    public void RemoveUserId(ObjectId urlId, long userId, CancellationToken cancellationToken)
    {
        var update = Builders<UriToVacancies>.Update.Pull(url => url.UserIds, userId);
        _mongoContext.UriToVacanciesCollection.UpdateOne(url => url.Id == urlId, update, null, cancellationToken);
    }

    public UriToVacancies ReplaceIfNotExistCopy(long userId, UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        UriToVacancies? oldUrl = GetOrDefault(uriToVacancies.HashedUrl, cancellationToken);
        if (oldUrl is null)
        {
            Replace(uriToVacancies, cancellationToken);
            return uriToVacancies;
        }
        else
        {
            AddUserId(oldUrl.Id, userId, cancellationToken);
            RemoveUserId(uriToVacancies.Id, userId, cancellationToken); 
            return oldUrl;
        }
    }

    public void Activate(ObjectId urlId, bool isActivate, CancellationToken cancellationToken)
    {
        var update = Builders<UriToVacancies>.Update.Set(url => url.IsActivate, isActivate);
        _mongoContext.UriToVacanciesCollection.UpdateOne(url => url.Id == urlId, update, null, cancellationToken);
    }
}
