using Deployf.Botf;
using Parsers.Constants;
using TgBotForSearchWork.Constants;
using TgBotForSearchWork.Services;
using TgBotForSearchWorkApi.Constants;

namespace TgBotForSearchWorkApi.Controllers;

public class UrlToVacanciesController : BaseController
{
    private readonly UserService _userService;

    public UrlToVacanciesController(UserService userService)
    {
        _userService = userService;
    }

    [Action(Command.ShowUrls, CommandDescription.Empty)]
    public void ShowUrls()
    {
        ShowSiteNames(GetUrlToVacancies);
    }

    [Action(Command.RemoveUrl, CommandDescription.Empty)]
    public void RemoveUrl()
    {
        ShowSiteNames(RemoveUrlToVacancies);
    }

    [Action]
    public void ShowSiteNames(Delegate next)
    {
        Push("Виберіть, назву сайту.");
        foreach (var siteType in Enum.GetValues<SiteType>())
        {
            RowButton(siteType.ToString(), Q(ShowUrlsToVacancies, (int)siteType, next));
        }
    }

    [Action]
    private void ShowUrlsToVacancies(int siteType, Delegate next)
    {
        Push("Виберіть, потрібне посилання.");
        foreach (var indexToUrl in _userService.GetUserUrlsToVacancies(ChatId, siteType, CancelToken))
        {
            RowButton(indexToUrl.Value.WithOutHttps, Q(next, indexToUrl.Key));
        }
    }

    [Action]
    private void GetUrlToVacancies(int index)
    {
        Send(_userService.GetUserUrlToVacancies(ChatId, index, CancelToken).OriginalString);
    }

    [Action]
    private void RemoveUrlToVacancies(int index)
    {
        _userService.RemoveUrlToVacancyAtUser(ChatId, index, CancelToken);
        Send("Посилання видалено.");
    }
}
