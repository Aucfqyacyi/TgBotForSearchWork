using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using Parsers.VacancyParsers;
using Telegram.Bot.Types;
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

    public UriToVacancies Create(long chatId, SiteType siteType, GetParametr getParametr, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = new(chatId, SiteTypesToUris.All[siteType].OriginalString);
        uriToVacancies.AddGetParametr(getParametr.Name, getParametr.Value);
        _uriToVacanciesRepository.InsertOne(uriToVacancies, cancellationToken);
        return uriToVacancies;
    }

    public UriToVacancies Update(ObjectId urlId, GetParametr getParametr, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = _uriToVacanciesRepository.Pop(urlId, cancellationToken);
        uriToVacancies.AddGetParametr(getParametr.Name, getParametr.Value);
        _uriToVacanciesRepository.InsertOne(uriToVacancies, cancellationToken);
        return uriToVacancies;
    }

    public void Activate(ObjectId urlId, bool isActivate, CancellationToken cancellationToken)
    {
        _uriToVacanciesRepository.Activate(urlId, isActivate, cancellationToken);
    }

    public async Task<UriToVacancies?> AddAsync(long chatId, string url, CancellationToken cancellationToken)
    {
        try
        {
            if (url.IsUrl() is false)
                return null;
            Uri uri = new Uri(url);
            IVacancyParser vacancyParser = VacancyParserFactory.CreateVacancyParser(uri);
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
}
