using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using Parsers.VacancyParsers;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class UriToVacanciesService
{
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;

    public UriToVacanciesService(UriToVacanciesRepository uriToVacanciesRepository)
    {
        _uriToVacanciesRepository = uriToVacanciesRepository;
    }

    public List<UriToVacancies> GetAll(long chatId, SiteType siteType, CancellationToken cancellationToken)
    {        
        return _uriToVacanciesRepository.GetAll(chatId, siteType, cancellationToken);
    }

    public UriToVacancies Get(ObjectId uriId, CancellationToken cancellationToken)
    {
        return _uriToVacanciesRepository.Get(uriId, cancellationToken);
    }

    public UriToVacancies? Create(long chatId, SiteType siteType, GetParameter getParametr, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = new(chatId, SiteTypesToUris.All[siteType].OriginalString);
        uriToVacancies.AddGetParameter(getParametr);
        _uriToVacanciesRepository.InsertOne(uriToVacancies, cancellationToken);
        return uriToVacancies;
    }

    public UriToVacancies? Update(ObjectId urlId, GetParameter getParametr, bool addGetParameter, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = _uriToVacanciesRepository.Get(urlId, cancellationToken);
        if(addGetParameter)
            uriToVacancies.AddGetParameter(getParametr);
        else
            uriToVacancies.RemoveGetParameter(getParametr);

        _uriToVacanciesRepository.Replace(uriToVacancies, cancellationToken);
        return uriToVacancies;
    }

    public void Activate(ObjectId urlId, bool isActivated, CancellationToken cancellationToken)
    {
        _uriToVacanciesRepository.Activate(urlId, isActivated, cancellationToken);
    }

    public async ValueTask<UriToVacancies?> AddAsync(long chatId, string url, CancellationToken cancellationToken)
    {
        try
        {
            if (url.IsUrl() is false)
                return null;
            Uri uri = new Uri(url);
            IVacancyParser vacancyParser = VacancyParserFactory.Create(uri);
            if (await vacancyParser.IsCorrectUrlAsync(uri, cancellationToken) is false)
                return null;
            UriToVacancies uriToVacancies = new(chatId, uri);
            _uriToVacanciesRepository.InsertOne(uriToVacancies, cancellationToken);
            return uriToVacancies;
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
        return null;
    }

    public void Delete(ObjectId urlId, CancellationToken cancellationToken)
    {
        _uriToVacanciesRepository.Delete(urlId, cancellationToken);
    }

    public bool IsActivated(ObjectId urlId, CancellationToken cancellationToken) 
    {
        return _uriToVacanciesRepository.IsActivated(urlId, cancellationToken);
    }
}
