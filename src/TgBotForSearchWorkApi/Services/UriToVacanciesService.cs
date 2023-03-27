using AutoDIInjector.Attributes;
using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using Parsers.ParserFactories;
using Parsers.VacancyParsers;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class UriToVacanciesService
{
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;
    private readonly VacancyParserFactory _vacancyParserFactory;

    public UriToVacanciesService(UriToVacanciesRepository uriToVacanciesRepository, VacancyParserFactory vacancyParserFactory)
    {
        _uriToVacanciesRepository = uriToVacanciesRepository;
        _vacancyParserFactory = vacancyParserFactory;
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

    public ValueTask<UriToVacancies> RemoveFilterAsync(ObjectId urlId, IEnumerable<string> getParameterNames, CancellationToken cancellationToken)
    {
        return UpdateAsync(urlId, getParameterNames, UriToVacancies.RemoveGetParameter, cancellationToken);
    }

    private async ValueTask<UriToVacancies> UpdateAsync<TValue>(ObjectId urlId, TValue value, Action<UriToVacancies, TValue> updateAction, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = await _uriToVacanciesRepository.GetAsync(urlId, cancellationToken);
        updateAction(uriToVacancies, value);
        await _uriToVacanciesRepository.ReplaceAsync(uriToVacancies, cancellationToken);
        return uriToVacancies;
    }

    public async ValueTask<UriToVacancies?> AddAsync(long chatId, string url, CancellationToken cancellationToken)
    {
        try
        {
            if (url.IsUrl() is false)
                return null;
            Uri uri = new(url);
            IVacancyParser vacancyParser = _vacancyParserFactory.GetOrCreate(uri);
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
}
