using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using Telegram.Bot.Types;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
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

    public UriToVacancies Get(ObjectId objectId, CancellationToken cancellationToken)
    {
        return _uriToVacanciesRepository.Get(objectId, cancellationToken);
    }

    public UriToVacancies Create(long userId, SiteType siteType, GetParametr getParametr, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = new(userId, SiteTypesToUris.All[siteType].OriginalString);
        uriToVacancies.AddGetParametr(getParametr.Name, getParametr.Value);
        return _uriToVacanciesRepository.InsertOrUpdate(userId, uriToVacancies, cancellationToken);
    }

    public UriToVacancies Update(long userId, ObjectId urlId, GetParametr getParametr, CancellationToken cancellationToken)
    {
        UriToVacancies uriToVacancies = _uriToVacanciesRepository.Get(urlId, cancellationToken);
        uriToVacancies.AddGetParametr(getParametr.Name, getParametr.Value);
        return _uriToVacanciesRepository.ReplaceIfNotExistCopy(userId, uriToVacancies, cancellationToken);
    }

    public void Activate(ObjectId urlId, bool isActivate, CancellationToken cancellationToken)
    {
        _uriToVacanciesRepository.Activate(urlId, isActivate, cancellationToken);
    }

    public void RemoveUserId(ObjectId urlId, long userId, CancellationToken cancellationToken)
    {
        _uriToVacanciesRepository.RemoveUserId(urlId, userId, cancellationToken);
    }
}
