using Parsers.Constants;
using Parsers.VacancyParsers;
using System.Text;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWork.Services;

[SingletonService]
public class UserService
{
    private readonly UserRepository _userRepository;

    public UserService(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Dictionary<int, UrlToVacancies> GetUserUrlsToVacancies(long chatId, SiteType siteType, CancellationToken cancellationToken)
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

    public UrlToVacancies GetUserUrlToVacancies(long chatId, int index, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        return user.Urls[index];
    }

    public async Task<bool> AddUrlToVacancyToUserAsync(long chatId, string url, CancellationToken cancellationToken)
    {       
        try
        {
            Uri uri = new Uri(url);
            IVacancyParser vacancyParser = VacancyParserFactory.CreateVacancyParser(uri);
            if (await vacancyParser.IsCorrectUrlAsync(uri, cancellationToken))
            {
                User user = _userRepository.Get(chatId, cancellationToken);
                user.AddUrlToVacancias(new(uri));
                _userRepository.Update(user, cancellationToken);
                return true;
            }          
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
        return false;
    }

    public void RemoveUrlToVacancyAtUser(long chatId, int index, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        user.RemoveUrlToVacancias(index);
        _userRepository.Update(user, cancellationToken);
    }
}
