﻿using AutoDIInjector.Attributes;
using MongoDB.Bson;
using MongoDB.Driver;
using Parsers.Constants;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;

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

    private FilterDefinition<UriToVacancies> GetFilterByChatId(long chatId)
    {
        return Builders<UriToVacancies>.Filter.Eq(url => url.ChatId, chatId);
    }

    private FilterDefinition<UriToVacancies> GetFilterByIsActivated()
    {
        return Builders<UriToVacancies>.Filter.Eq(url => url.IsActivated, true);
    }

    public void InsertMany(IReadOnlyList<UriToVacancies> uriToVacancies, CancellationToken cancellationToken)
    {
        _mongoContext.UriToVacanciesCollection.InsertMany(uriToVacancies, null, cancellationToken);
    }

    public long Count(long chatId, CancellationToken cancellationToken)
    {
        return _mongoContext.UriToVacanciesCollection.CountDocuments(GetFilterByChatId(chatId), null, cancellationToken);
    }

    public void InsertOne(UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        _mongoContext.UriToVacanciesCollection.InsertOne(uriToVacancies, null, cancellationToken);
    }

    public List<UriToVacancies> GetAll(long chatId, SiteType siteType, CancellationToken cancellationToken)
    {
        var chatIdFilter = GetFilterByChatId(chatId);
        var siteTypeFilter = Builders<UriToVacancies>.Filter.Eq(url => url.SiteType, siteType);
        return _mongoContext.UriToVacanciesCollection.FindSync(chatIdFilter & siteTypeFilter, null, cancellationToken)
                                                     .ToList();
    }

    public List<UriToVacancies> GetAllActivated(long chatId, CancellationToken cancellationToken)
    {
        return _mongoContext.UriToVacanciesCollection.FindSync(GetFilterByChatId(chatId) & GetFilterByIsActivated(), null, cancellationToken)
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

    public bool IsActivated(ObjectId urlId, CancellationToken cancellationToken)
    {
        var options = new FindOptions<UriToVacancies, bool>();
        options.Projection = new ProjectionDefinitionBuilder<UriToVacancies>().Expression(uri => uri.IsActivated);
        return _mongoContext.UriToVacanciesCollection.FindSync(GetFilterById(urlId), options, cancellationToken).FirstOrDefault();
    }

    public void UpdateManyLastVacancyIds(IEnumerable<UriToVacancies> urisToVacancies, CancellationToken cancellationToken)
    {
        var requests = urisToVacancies.Aggregate(new List<WriteModel<UriToVacancies>>(), (requests, uri) =>
        {
            var update = Builders<UriToVacancies>.Update.Set(url => url.LastVacanciesIds, uri.LastVacanciesIds);
            requests.Add(new UpdateOneModel<UriToVacancies>(GetFilterById(uri.Id), update));
            return requests;
        });
        _mongoContext.UriToVacanciesCollection.BulkWrite(requests, null, cancellationToken);
    }
}