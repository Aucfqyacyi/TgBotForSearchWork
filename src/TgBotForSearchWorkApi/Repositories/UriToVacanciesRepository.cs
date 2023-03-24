using AutoDIInjector.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Parsers.Constants;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Repositories;

[SingletonService]
public class UriToVacanciesRepository
{
    private readonly IMongoDbContext _mongoContext;

    public UriToVacanciesRepository(IMongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    private FilterDefinition<UriToVacancies> GetFilterById(ObjectId urlId)
    {
        return Builders<UriToVacancies>.Filter.Eq(url => url.Id, urlId);
    }

    private FilterDefinition<UriToVacancies> GetFilterByChatId(long chatId)
    {
        return Builders<UriToVacancies>.Filter.Eq(url => url.ChatId, chatId);
    }

    private FilterDefinition<UriToVacancies> GetFilterByIsActivated()
    {
        return Builders<UriToVacancies>.Filter.Eq(url => url.IsActivated, true);
    }

    public ValueTask InsertManyAsync(IReadOnlyList<UriToVacancies> uriToVacancies, CancellationToken cancellationToken)
    {
        return new(_mongoContext.UriToVacanciesCollection.InsertManyAsync(uriToVacancies, null, cancellationToken));
    }

    public ValueTask<long> CountAsync(long chatId, CancellationToken cancellationToken)
    {
        return new(_mongoContext.UriToVacanciesCollection.CountDocumentsAsync(GetFilterByChatId(chatId), null, cancellationToken));
    }

    public ValueTask InsertOneAsync(UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        return new(_mongoContext.UriToVacanciesCollection.InsertOneAsync(uriToVacancies, null, cancellationToken));
    }

    public async ValueTask<List<UriToVacancies>> GetAllAsync(long chatId, SiteType siteType, CancellationToken cancellationToken)
    {
        var chatIdFilter = GetFilterByChatId(chatId);
        var siteTypeFilter = Builders<UriToVacancies>.Filter.Eq(url => url.SiteType, siteType);
        var cursor = await _mongoContext.UriToVacanciesCollection.FindAsync(chatIdFilter & siteTypeFilter, null, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async ValueTask<List<UriToVacancies>> GetAllActivatedAsync(long chatId, CancellationToken cancellationToken)
    {
        var cursor = await _mongoContext.UriToVacanciesCollection.FindAsync(GetFilterByChatId(chatId) & GetFilterByIsActivated(), null, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public async ValueTask<UriToVacancies> GetAsync(ObjectId urlId, CancellationToken cancellationToken)
    {
        UriToVacancies? uriToVacancies = await GetOrDefaultAsync(urlId, cancellationToken);
        return uriToVacancies ?? throw new Exception($"Uri with id({urlId}) doesn't exist.");
    }

    public async ValueTask<UriToVacancies?> GetOrDefaultAsync(ObjectId urlId, CancellationToken cancellationToken)
    {
        var cursor = await _mongoContext.UriToVacanciesCollection.FindAsync(GetFilterById(urlId), null, cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public ValueTask ReplaceAsync(UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        ReplaceOptions? replaceOptions = null;
        return new(_mongoContext.UriToVacanciesCollection.ReplaceOneAsync(GetFilterById(uriToVacancies.Id), uriToVacancies,
                                                                                        replaceOptions, cancellationToken));
    }

    public ValueTask DeleteAsync(ObjectId urlId, CancellationToken cancellationToken)
    {
        return new(_mongoContext.UriToVacanciesCollection.DeleteOneAsync(GetFilterById(urlId), cancellationToken));
    }

    public ValueTask ActivateAsync(ObjectId urlId, bool isActivated, CancellationToken cancellationToken)
    {
        var update = Builders<UriToVacancies>.Update.Set(url => url.IsActivated, isActivated);
        return new(_mongoContext.UriToVacanciesCollection.UpdateOneAsync(GetFilterById(urlId), update, null, cancellationToken));
    }

    public async ValueTask<bool> IsActivatedAsync(ObjectId urlId, CancellationToken cancellationToken)
    {
        var options = new FindOptions<UriToVacancies, bool>();
        options.Projection = new ProjectionDefinitionBuilder<UriToVacancies>().Expression(uri => uri.IsActivated);
        var cursor = await _mongoContext.UriToVacanciesCollection.FindAsync(GetFilterById(urlId), options, cancellationToken);
        return await cursor.FirstOrDefaultAsync(cancellationToken);
    }

    public async ValueTask<List<SiteType>> GetAllAsSiteTypesAsync(long chatId, CancellationToken cancellationToken)
    {
        var options = new FindOptions<UriToVacancies, SiteType>();
        options.Projection = new ProjectionDefinitionBuilder<UriToVacancies>().Expression(uri => uri.SiteType);
        var cursor = await _mongoContext.UriToVacanciesCollection.FindAsync(GetFilterByChatId(chatId), options, cancellationToken);
        return await cursor.ToListAsync(cancellationToken);
    }

    public ValueTask UpdateManyLastVacancyIdsAsync(IEnumerable<UriToVacancies> urisToVacancies, CancellationToken cancellationToken)
    {
        var requests = urisToVacancies.Aggregate(new List<WriteModel<UriToVacancies>>(), (requests, uri) =>
        {
            var update = Builders<UriToVacancies>.Update.Set(url => url.LastVacanciesIds, uri.LastVacanciesIds);
            requests.Add(new UpdateOneModel<UriToVacancies>(GetFilterById(uri.Id), update));
            return requests;
        });
        return new(_mongoContext.UriToVacanciesCollection.BulkWriteAsync(requests, null, cancellationToken));
    }
}
