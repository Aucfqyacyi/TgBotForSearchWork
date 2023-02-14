using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using System.Reflection;
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

    protected void GetSiteNames(Func<SiteType, string> q)
    {
        Push("Виберіть, назву сайту.");
        foreach (var siteType in Enum.GetValues<SiteType>())
        {
            RowButton(siteType.ToString(), q(siteType));
        }
    }

    protected void ActivateRowButton(ObjectId? urlId, bool? isActivate, bool sendNewMessage = true, Delegate? @delegate = null, params object[] args)
    {
        if (urlId is null || isActivate is null)
            return;
        string activatePhrase = isActivate.Value ? "Дезактивувати" : "Активувати";
        RowButton(activatePhrase, Q(ActivateUrl, urlId, !isActivate, sendNewMessage, @delegate, args));
    }

    [Action]
    protected async Task ActivateUrl(ObjectId urlId, bool isActivate, bool sendNewMessage, Delegate? @delegate = null, params object[] args)
    {
        _urlToVacanciesService.Activate(urlId, isActivate, CancelToken);
        string activatePhrase = "Посилання " + (isActivate ?  "активоване." : "дезактивоване.");
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
