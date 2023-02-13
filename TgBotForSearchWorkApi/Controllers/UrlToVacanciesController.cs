using Deployf.Botf;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Services;
using MongoDB.Bson;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UrlToVacanciesController : BotController
{
    protected readonly string Back = "Назад";
    protected readonly string FirstPage = "{0}";

    protected readonly UrlToVacanciesService _urlToVacanciesService;
    protected readonly FilterService _filterService;
    protected readonly UserService _userService;

    public UrlToVacanciesController(FilterService filterService, UrlToVacanciesService urlToVacanciesService, UserService userService)
    {
        _filterService = filterService;
        _urlToVacanciesService = urlToVacanciesService;
        _userService = userService;
    }

    [Action(Command.GetUrl, CommandDescription.Empty)]
    public void GetUrl()
    {
        GetSiteNamesThenGetUrlsToVacancies(GetUrlToVacancies);
    }

    [Action(Command.RemoveUrl, CommandDescription.Empty)]
    public void RemoveUrl()
    {
        GetSiteNamesThenGetUrlsToVacancies(RemoveUrlToVacancies);
    }

    [Action(Command.AddUrl, CommandDescription.Empty)]
    public void AddUrl()
    {
        State(new AddingUrlState());
        Push("Напишіть посилання у повному форматі, яке бажаєте додати.");
    }

    [Action]
    private void GetSiteNamesThenGetUrlsToVacancies(Delegate next)
    {
        GetSiteNames(siteType => Q(GetUrlsToVacancies, 0, siteType, next));
    }

    [Action]
    private void GetUrlsToVacancies(int page, SiteType siteType, Delegate next)
    {
        Push($"Виберіть, потрібне посилання.");
        List<UrlToVacancies> indexsToUrls = _urlToVacanciesService.GetAll(ChatId, siteType, CancelToken);
        Pager(indexsToUrls, page, indexToUrl => (indexToUrl.WithoutHttps, Q(next, indexToUrl.Id)),
                                        Q(GetUrlsToVacancies, FirstPage, siteType, next), 1);
        RowButton(Back, Q(GetSiteNamesThenGetUrlsToVacancies, next));
    }

    [Action]
    private async Task GetUrlToVacancies(ObjectId urlId)
    {
        await AnswerCallback();
        UrlToVacancies urlToVacancies = _urlToVacanciesService.Get(urlId, CancelToken);
        string activatePhrase = urlToVacancies.IsActivate? "Дезактивувати" : "Активувати";
        RowButton(activatePhrase, Q(ActivateUrl, 0, !urlToVacancies.IsActivate));
        await Send(urlToVacancies.OriginalString, new() { DisableWebPagePreview = true });
    }

    [Action]
    private async Task RemoveUrlToVacancies(int index)
    {
        //_userService.RemoveUrlToVacancy(ChatId, index, CancelToken);
        await Send("Посилання видалено.");
    }

    [State]
    private async Task HandleAddingUrlAsync(AddingUrlState state)
    {
        int index = 0;//await _userService.AddUrlToVacancyAsync(ChatId, Context.GetSafeTextPayload()!, CancelToken);
        if (index != default)
        {
            Push("Посилання було добавленно.");
            RowButton("Aктивувати", Q(ActivateUrl, index, true));
        }
        else
            Push("Посилання не корректне, або не містить жодної вакансії.");
    }

    [Action]
    private void GetSiteNames(Func<SiteType, string> q)
    {
        Push("Виберіть, назву сайту.");
        foreach (var siteType in Enum.GetValues<SiteType>())
        {
            RowButton(siteType.ToString(), q(siteType));
        }
    }

    [Action]
    private async Task ActivateUrl(int index, bool isActivate)
    {
        await AnswerCallback();
        //_userService.ActivateUrlToVacancy(ChatId, index, isActivate, CancelToken);
        if (isActivate)
            await Send("Посилання активоване.");
        else
            await Send("Посилання дезактивоване.");
    }

}
