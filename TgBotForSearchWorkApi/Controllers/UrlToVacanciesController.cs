using Deployf.Botf;
using Parsers.Constants;
using TgBotForSearchWork.Constants;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Services;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Utilities;

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

    [Action(Command.AddUrl, CommandDescription.Empty)]
    public void AddUrl()
    {
        State(new AddUrlState());
        Push("Напишіть посилання у повному форматі, яке бажаєте додати.");
    }

    [State]
    private async Task HandleAddingUrl(AddUrlState state)
    {
        int index = await _userService.AddUrlToVacancyAsync(ChatId, Context.GetSafeTextPayload()!, CancelToken);
        if (index != default)
        {
            Push("Посилання було добавленно.");
            RowButton("Aктивувати", Q(ActivateUrl, index, true));
        }
        else
            Push("Посилання не корректне, або не містить жодної вакансії.");
    }

    [Action]
    private void ActivateUrl(int index, bool isActivate)
    {
        _userService.ActivateUrlToVacancy(ChatId, index, isActivate, CancelToken);
        if(isActivate)
            Send("Посилання активоване.");
        else
            Send("Посилання дезактивоване.");
    }

    [Action]
    private void ShowSiteNames(Delegate next)
    {
        Push("Виберіть, назву сайту.");
        foreach (var siteType in Enum.GetValues<SiteType>())
        {
            RowButton(siteType.ToString(), Q(ShowUrlsToVacancies, 0, siteType, next));
        }
        Send();
    }

    [Action]
    private void ShowUrlsToVacancies(int page, SiteType siteType, Delegate next)
    {
        Push($"Виберіть, потрібне посилання");
        Dictionary<int, UrlToVacancies> indexsToUrls = _userService.GetUrlsToVacancies(ChatId, siteType, CancelToken);
        var pager = new PagingService();
        var query = indexsToUrls.AsQueryable();
        var pageModel = pager.Paging(query, new PaginationFilter(page));
        Pager(pageModel, indexToUrl => (indexToUrl.Value.WithOutHttps, Q(next, indexToUrl.Key)), 
                                        Q(ShowUrlsToVacancies, "{0}", siteType, next), 1);
        Send();
    }

    [Action]
    private void GetUrlToVacancies(int index)
    {
        UrlToVacancies urlToVacancies = _userService.GetUrlToVacancies(ChatId, index, CancelToken);
        if(urlToVacancies.IsActivate)
            RowButton("Дезактивувати", Q(ActivateUrl, index, false));
        else
            RowButton("Активувати", Q(ActivateUrl, index, true));        
        Push(urlToVacancies.OriginalString);
        Send();
    }

    [Action]
    private void RemoveUrlToVacancies(int index)
    {
        _userService.RemoveUrlToVacancy(ChatId, index, CancelToken);
        Send("Посилання видалено.");
    }
}
