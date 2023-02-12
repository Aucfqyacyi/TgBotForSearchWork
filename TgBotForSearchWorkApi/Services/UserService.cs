using Deployf.Botf;
using Parsers.Constants;
using Parsers.Models;
using Parsers.VacancyParsers;
using System.Web;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Dictionary<int, UrlToVacancies> GetUrlsToVacancies(long chatId, SiteType siteType, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        string host = SiteTypesToUris.All[siteType].Host;
        var indexsToUrls = new Dictionary<int, UrlToVacancies>();
        for (int i = 0; i < user.Urls.Count; i++)
        {
            if (user.Urls[i].Host == host)
                indexsToUrls.Add(i, user.Urls[i]);
        }
        return indexsToUrls;
    }

    public UrlToVacancies GetUrlToVacancies(long chatId, int index, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        return user.Urls[index];
    }

    public async Task<int> AddUrlToVacancyAsync(long chatId, string url, CancellationToken cancellationToken)
    {       
        try
        {
            if (url.IsUrl() is false)
                return default;
            Uri uri = new Uri(url);
            IVacancyParser vacancyParser = VacancyParserFactory.CreateVacancyParser(uri);
            if (await vacancyParser.IsCorrectUrlAsync(uri, cancellationToken))
            {
                User user = _userRepository.Get(chatId, cancellationToken);
                user.AddUrlToVacancias(new(uri));
                _userRepository.Update(user, cancellationToken);
                return user.Urls.Count - 1;
            }          
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
        return default;
    }

    public int CreateOrUpdateUrlToVacancies(long chatId, int urlIndex, SiteType siteType, Filter filter, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        if (urlIndex == 0)
        {
            user.AddUrlToVacancias(new(SiteTypesToUris.All[siteType]));
            urlIndex = user.Urls.Count - 1;
        }
        UrlToVacancies urlToVacancies = user.Urls[urlIndex];
        UriBuilder uriBuilder = new(urlToVacancies.Uri);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query.Add(filter.GetParametrName, filter.GetParametrValue);
        uriBuilder.Query = query.ToString();
        urlToVacancies.Uri = uriBuilder.Uri;
        urlToVacancies.IsActivate = false;
        _userRepository.Update(user, cancellationToken);
        return urlIndex;
    }

    public void RemoveUrlToVacancy(long chatId, int index, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        user.RemoveUrlToVacancias(index);
        _userRepository.Update(user, cancellationToken);
    }

    public void ActivateUrlToVacancy(long chatId, int index, bool isActivate, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        user.Urls[index].IsActivate = isActivate;
        _userRepository.Update(user, cancellationToken);
    }


}
