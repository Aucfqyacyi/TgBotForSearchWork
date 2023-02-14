using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using System.Reflection;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Services;

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
        Push("Напишіть посилання, яке бажаєте додати, у повному форматі.");
    }

    [Action]
    private void GetSiteNamesThenGetUrlsToVacancies(Delegate next)
    {
        GetSiteNames(siteType => Q(GetUrlsToVacanciesAsync, 0, siteType, next));
    }

    [Action]
    private async Task GetUrlsToVacanciesAsync(int page, SiteType siteType, Delegate next)
    {
        
        List<UrlToVacancies> urlsToVacancies = _urlToVacanciesService.GetAll(ChatId, siteType, CancelToken);
        if (urlsToVacancies.Count <= 0)
        {
            await Send("У вас немає посилань.");
            return;
        }
        Push("Виберіть, потрібне посилання.");
        Pager(urlsToVacancies, page, indexToUrl => (indexToUrl.WithoutHttps, Q(next, indexToUrl.Id)),
                                        Q(GetUrlsToVacanciesAsync, FirstPage, siteType, next), 1);
        RowButton(Back, Q(GetSiteNamesThenGetUrlsToVacancies, next));
    }

    [Action]
    private async Task GetUrlToVacancies(ObjectId urlId)
    {
        await AnswerCallback();
        UrlToVacancies urlToVacancies = _urlToVacanciesService.Get(urlId, CancelToken);
        ActivateRowButton(urlToVacancies.Id, urlToVacancies.IsActivate);
        await Send(urlToVacancies.OriginalString, new() { DisableWebPagePreview = true });
    }

    [Action]
    private async Task RemoveUrlToVacancies(ObjectId urlId)
    {
        _userService.RemoveUrlToVacancy(ChatId, urlId, CancelToken);
        await Send("Посилання видалено.");
    }

    [State]
    private async Task HandleAddingUrlAsync(AddingUrlState state)
    {
        UrlToVacancies? urlToVacancies = await _userService.AddUrlToVacancyAsync(ChatId, Context.GetSafeTextPayload()!, CancelToken);
        ActivateRowButton(urlToVacancies?.Id, urlToVacancies?.IsActivate);
        if (urlToVacancies is not null)
            Push("Посилання було добавленно.");
        else
            Push("Посилання не корректне, або не містить жодної вакансії, або вже добавленно.");
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

    private void ActivateRowButton(ObjectId? urlId, bool? isActivate, bool sendNewMessage = true, Delegate? @delegate = null, params object[] args)
    {
        if (urlId is null || isActivate is null)
            return;
        string activatePhrase = isActivate.Value ? "Дезактивувати" : "Активувати";
        RowButton(activatePhrase, Q(ActivateUrl, urlId, !isActivate, sendNewMessage, @delegate, args));
    }

    [Action]
    private async Task ActivateUrl(ObjectId urlId, bool isActivate, bool sendNewMessage, Delegate? @delegate = null, params object[] args)
    {       
        _urlToVacanciesService.Activate(urlId, isActivate, CancelToken);
        string activatePhrase = "Посилання " + (isActivate ? "дезактивувати." : "активоване.");
        if (sendNewMessage)
        {
            await AnswerCallback();
            await Send(activatePhrase);
        }
        else
        {
            await AnswerOkCallback();
            if (@delegate is not null)
            {
                MethodInfo methodInfo = @delegate.GetMethodInfo();
                methodInfo.Invoke(this, new List<object?>(args) { isActivate }.ToArray());
            }
        }
            
    }



}
