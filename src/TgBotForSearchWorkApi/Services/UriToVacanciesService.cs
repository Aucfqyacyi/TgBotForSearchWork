using AutoDIInjector.Attributes;
using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using Parsers.VacancyParsers;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class UriToVacanciesService
{
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;

    public UriToVacanciesService(UriToVacanciesRepository uriToVacanciesRepository)
    {
        _uriToVacanciesRepository = uriToVacanciesRepository;
    }

    public ValueTask<List<UriToVacancies>> GetAllAsync(long chatId, SiteType siteType, CancellationToken cancellationToken)
    {
        return _uriToVacanciesRepository.GetAllAsync(chatId, siteType, cancellationToken);
    }

    public ValueTask<UriToVacancies> GetAsync(ObjectId uriId, CancellationToken cancellationToken)
    {
        return _uriToVacanciesRepository.GetAsync(uriId, cancellationToken);
    }

    public async ValueTask<UriToVacancies?> CreateAsync(long chatId, SiteType siteType, GetParameter getParametr, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = new(chatId, SiteTypesToUris.All[siteType], siteType);
        uriToVacancies.AddGetParameter(getParametr);
        await _uriToVacanciesRepository.InsertOneAsync(uriToVacancies, cancellationToken);
        return uriToVacancies;
    }

    public ValueTask<UriToVacancies> AddFilterAsync(ObjectId urlId, GetParameter getParametr, CancellationToken cancellationToken)
    {
        return UpdateAsync(urlId, getParametr, UriToVacancies.AddGetParameter, cancellationToken);
    }

    public ValueTask<UriToVacancies> RemoveFilterAsync(ObjectId urlId, GetParameter getParametr, CancellationToken cancellationToken)
    {
        return UpdateAsync(urlId, getParametr, UriToVacancies.RemoveGetParameter, cancellationToken);
    }

    private async ValueTask<UriToVacancies> UpdateAsync(ObjectId urlId, GetParameter getParametr, Action<UriToVacancies, GetParameter> updateAction, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = await _uriToVacanciesRepository.GetAsync(urlId, cancellationToken);
        updateAction(uriToVacancies, getParametr);
        await _uriToVacanciesRepository.ReplaceAsync(uriToVacancies, cancellationToken);
        return uriToVacancies;
    }

    public ValueTask ActivateAsync(ObjectId urlId, bool isActivated, CancellationToken cancellationToken)
    {
        return _uriToVacanciesRepository.ActivateAsync(urlId, isActivated, cancellationToken);
    }

    public async ValueTask<UriToVacancies?> AddAsync(long chatId, string url, CancellationToken cancellationToken)
    {
        try
        {
            if (url.IsUrl() is false)
                return null;
            Uri uri = new Uri(url);
            IVacancyParser vacancyParser = VacancyParserFactory.Create(uri);
            if (await vacancyParser.IsCorrectUriAsync(uri, cancellationToken) is false)
                return null;
            UriToVacancies uriToVacancies = new(chatId, uri);
            await _uriToVacanciesRepository.InsertOneAsync(uriToVacancies, cancellationToken);
            return uriToVacancies;
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
        return null;
    }

    public ValueTask DeleteAsync(ObjectId urlId, CancellationToken cancellationToken)
    {
        return _uriToVacanciesRepository.DeleteAsync(urlId, cancellationToken);
    }

    public ValueTask<bool> IsActivatedAsync(ObjectId urlId, CancellationToken cancellationToken)
    {
        return _uriToVacanciesRepository.IsActivatedAsync(urlId, cancellationToken);
    }

    public ValueTask<long> CountAsync(long chatId, CancellationToken cancellationToken)
    {
        return _uriToVacanciesRepository.CountAsync(chatId, cancellationToken);
    }
}
