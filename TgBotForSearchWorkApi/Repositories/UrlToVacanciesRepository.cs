using MongoDB.Bson;
using MongoDB.Driver;
using Parsers.Constants;
using System.Threading;
using Telegram.Bot.Types;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Repositories;

[SingletonService]
public class UrlToVacanciesRepository
{
    private readonly MongoDbContext _mongoContext;

    public UrlToVacanciesRepository(MongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    public void InsertMany(IReadOnlyList<UrlToVacancies> urlToVacancies, CancellationToken cancellationToken)
    {
        _mongoContext.UrlToVacanciesCollection.InsertMany(urlToVacancies, null, cancellationToken);  
    }

    public void InsertOne(UrlToVacancies urlToVacancies, CancellationToken cancellationToken)
    {
        _mongoContext.UrlToVacanciesCollection.InsertOne(urlToVacancies, null, cancellationToken);
    }

    public List<UrlToVacancies> GetAll(long userId, SiteType siteType, CancellationToken cancellationToken)
    {
        var userIdsFilter = Builders<UrlToVacancies>.Filter.AnyEq(url => url.UserIds, userId);
        var siteTypeFilter = Builders<UrlToVacancies>.Filter.Eq(url => url.SiteType, siteType);
        return _mongoContext.UrlToVacanciesCollection.FindSync(userIdsFilter & siteTypeFilter, null, cancellationToken)
                                                     .ToList();
    }

    public UrlToVacancies Get(ObjectId urlId, CancellationToken cancellationToken)
    {
        UrlToVacancies? urlToVacancies = GetOrDefault(urlId, cancellationToken);
        if (urlToVacancies is null)
            throw new Exception($"Url with id({urlId}) doesnot exist.");
        return urlToVacancies;
    }

    public UrlToVacancies? GetOrDefault(ObjectId urlId, CancellationToken cancellationToken)
    {
        return _mongoContext.UrlToVacanciesCollection.FindSync(url => url.Id == urlId, null, cancellationToken)
                                                     .FirstOrDefault(cancellationToken);
    }

    public void Replace(UrlToVacancies urlToVacancies, CancellationToken cancellationToken)
    {
        ReplaceOptions? replaceOptions = null;
        _mongoContext.UrlToVacanciesCollection.ReplaceOne(u => u.Id == urlToVacancies.Id, urlToVacancies, replaceOptions, cancellationToken);
    }

    public UrlToVacancies? GetOrDefault(string hashedUrl, CancellationToken cancellationToken)
    {
        return _mongoContext.UrlToVacanciesCollection.FindSync(url => url.HashedUrl == hashedUrl, null, cancellationToken)
                                                                    .FirstOrDefault(cancellationToken);
    }

    public UrlToVacancies InsertOrUpdate(long userId, UrlToVacancies urlToVacancies, CancellationToken cancellationToken)
    {
        UrlToVacancies? oldUrl = GetOrDefault(urlToVacancies.HashedUrl, cancellationToken);
        if (oldUrl is null)
        {
            InsertOne(urlToVacancies, cancellationToken);
            return urlToVacancies;
        }
        else
        {
            AddUserId(oldUrl.Id, userId, cancellationToken);
            return oldUrl;
        }
    }

    public void AddUserId(ObjectId urlId, long userId, CancellationToken cancellationToken)
    {
        var update = Builders<UrlToVacancies>.Update.AddToSet(url => url.UserIds, userId);
        _mongoContext.UrlToVacanciesCollection.UpdateOne(url => url.Id == urlId, update, null, cancellationToken);
    }

    public void RemoveUserId(ObjectId urlId, long userId, CancellationToken cancellationToken)
    {
        var update = Builders<UrlToVacancies>.Update.Pull(url => url.UserIds, userId);
        _mongoContext.UrlToVacanciesCollection.UpdateOne(url => url.Id == urlId, update, null, cancellationToken);
    }

    public UrlToVacancies ReplaceIfNotExistCopy(long userId, UrlToVacancies urlToVacancies, CancellationToken cancellationToken)
    {
        UrlToVacancies? oldUrl = GetOrDefault(urlToVacancies.HashedUrl, cancellationToken);
        if (oldUrl is null)
        {
            Replace(urlToVacancies, cancellationToken);
            return urlToVacancies;
        }
        else
        {
            AddUserId(oldUrl.Id, userId, cancellationToken);
            RemoveUserId(urlToVacancies.Id, userId, cancellationToken); 
            return oldUrl;
        }
    }

    public void Activate(ObjectId urlId, bool isActivate, CancellationToken cancellationToken)
    {
        var update = Builders<UrlToVacancies>.Update.Set(url => url.IsActivate, isActivate);
        _mongoContext.UrlToVacanciesCollection.UpdateOne(url => url.Id == urlId, update, null, cancellationToken);
    }
}
