using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using Telegram.Bot.Types;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class UrlToVacanciesService
{
    private readonly UrlToVacanciesRepository _urlToVacanciesRepository;

    public UrlToVacanciesService(UrlToVacanciesRepository urlToVacanciesRepository)
    {
        _urlToVacanciesRepository = urlToVacanciesRepository;
    }

    public List<UrlToVacancies> GetAll(long chatId, SiteType siteType, CancellationToken cancellationToken)
    {        
        return _urlToVacanciesRepository.GetAll(chatId, siteType, cancellationToken);
    }

    public UrlToVacancies Get(ObjectId objectId, CancellationToken cancellationToken)
    {
        return _urlToVacanciesRepository.Get(objectId, cancellationToken);
    }

    public UrlToVacancies Create(long userId, SiteType siteType, GetParametr getParametr, CancellationToken cancellationToken)
    {
        UrlToVacancies urlToVacancies = new(userId, SiteTypesToUris.All[siteType].OriginalString);
        urlToVacancies.AddGetParametr(getParametr.Name, getParametr.Value);
        return _urlToVacanciesRepository.InsertOrUpdate(userId, urlToVacancies, cancellationToken);
    }

    public UrlToVacancies Update(long userId, ObjectId urlId, GetParametr getParametr, CancellationToken cancellationToken)
    {
        UrlToVacancies urlToVacancies = _urlToVacanciesRepository.Get(urlId, cancellationToken);
        urlToVacancies.AddGetParametr(getParametr.Name, getParametr.Value);
        return _urlToVacanciesRepository.ReplaceIfNotExistCopy(userId, urlToVacancies, cancellationToken);
    }

    public void Activate(ObjectId urlId, bool isActivate, CancellationToken cancellationToken)
    {
        _urlToVacanciesRepository.Activate(urlId, isActivate, cancellationToken);
    }

    public void RemoveUserId(ObjectId urlId, long userId, CancellationToken cancellationToken)
    {
        _urlToVacanciesRepository.RemoveUserId(urlId, userId, cancellationToken);
    }
}
