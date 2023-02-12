using Deployf.Botf;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Services;


namespace TgBotForSearchWorkApi.Controllers;

public partial class UrlToVacanciesController : BotController
{
    protected readonly string Back = "Назад";
    protected readonly string FirstPage = "{0}";

    protected readonly UserService _userService;
    protected readonly FilterService _filterService;

    public UrlToVacanciesController(UserService userService, FilterService filterService)
    {
        _userService = userService;
        _filterService = filterService;
    }

    [Action(Command.GetUrl, CommandDescription.Empty)]
    public void GetUrl()
    {
        ShowSiteNamesThenShowUrlsToVacancies(GetUrlToVacancies);
    }

    [Action(Command.RemoveUrl, CommandDescription.Empty)]
    public void RemoveUrl()
    {
        ShowSiteNamesThenShowUrlsToVacancies(RemoveUrlToVacancies);
    }

    [Action(Command.AddUrl, CommandDescription.Empty)]
    public void AddUrl()
    {
        State(new AddUrlState());
        Push("Напишіть посилання у повному форматі, яке бажаєте додати.");
    }

    [Action]
    private void ShowSiteNamesThenShowUrlsToVacancies(Delegate next)
    {
        ShowSiteNames(siteType => Q(ShowUrlsToVacancies, 0, siteType, next));
    }

    [Action]
    private void ShowUrlsToVacancies(int page, SiteType siteType, Delegate next)
    {
        Push($"Виберіть, потрібне посилання.");
        Dictionary<int, UrlToVacancies> indexsToUrls = _userService.GetUrlsToVacancies(ChatId, siteType, CancelToken);
        Pager(indexsToUrls, page, indexToUrl => (indexToUrl.Value.WithOutHttps, Q(next, indexToUrl.Key)),
                                        Q(ShowUrlsToVacancies, FirstPage, siteType, next), 1);
        RowButton(Back, Q(ShowSiteNamesThenShowUrlsToVacancies, next));
    }

    [Action]
    private async Task GetUrlToVacancies(int index)
    {
        await AnswerCallback();
        UrlToVacancies urlToVacancies = _userService.GetUrlToVacancies(ChatId, index, CancelToken);
        if (urlToVacancies.IsActivate)
            RowButton("Дезактивувати", Q(ActivateUrl, index, false));
        else
            RowButton("Активувати", Q(ActivateUrl, index, true));
        await Send(urlToVacancies.OriginalString, new() { DisableWebPagePreview = true });
    }

    [Action]
    private async Task RemoveUrlToVacancies(int index)
    {
        _userService.RemoveUrlToVacancy(ChatId, index, CancelToken);
        await Send("Посилання видалено.");
    }

    [State]
    private async Task HandleAddingUrlAsync(AddUrlState state)
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
    protected void ShowSiteNames(Func<SiteType, string> q)
    {
        Push("Виберіть, назву сайту.");
        foreach (var siteType in Enum.GetValues<SiteType>())
        {
            RowButton(siteType.ToString(), q(siteType));
        }
    }

    [Action]
    protected async Task ActivateUrl(int index, bool isActivate)
    {
        await AnswerCallback();
        _userService.ActivateUrlToVacancy(ChatId, index, isActivate, CancelToken);
        if (isActivate)
            await Send("Посилання активоване.");
        else
            await Send("Посилання дезактивоване.");
    }

}
