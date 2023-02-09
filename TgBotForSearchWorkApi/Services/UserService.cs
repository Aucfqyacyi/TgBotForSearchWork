using Parsers.Constants;
using System.Text;
using TgBotForSearchWork.Models;
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

    public Dictionary<int, UrlToVacancies> GetUserUrlsToVacancies(long chatId, int siteType, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        string host = SiteTypesToUris.All[(SiteType)siteType].Host;
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

    public void AddUrlToVacancyToUser(long chatId, UrlToVacancies urlToVacancies, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        user.AddUrlToVacancias(urlToVacancies);
        _userRepository.Update(user, cancellationToken);
    }

    public void RemoveUrlToVacancyAtUser(long chatId, int index, CancellationToken cancellationToken)
    {
        User user = _userRepository.Get(chatId, cancellationToken);
        user.RemoveUrlToVacancias(index);
        _userRepository.Update(user, cancellationToken);
    }
}
