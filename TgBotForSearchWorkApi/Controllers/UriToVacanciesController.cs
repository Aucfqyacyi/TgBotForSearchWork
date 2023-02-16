using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using System.Reflection;
using TgBotForSearchWorkApi.Services;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController : BotController
{
    protected readonly string Back = "Назад";
    protected readonly string FirstPage = "{0}";

    protected readonly UriToVacanciesService _uriToVacanciesService;
    protected readonly FilterService _filterService;

    public UriToVacanciesController(FilterService filterService, UriToVacanciesService uriToVacanciesService)
    {
        _filterService = filterService;
        _uriToVacanciesService = uriToVacanciesService;
    }

    protected void GetSiteNames(Func<SiteType, string> q)
    {
        Push("Виберіть, назву сайту.");
        foreach (var siteType in Enum.GetValues<SiteType>())
        {
            RowButton(siteType.ToString(), q(siteType));
        }
    }

    protected void ActivateRowButton(ObjectId? urlId, bool? isActivated, bool sendNewMessage = true, Delegate? @delegate = null, params object[] args)
    {
        if (urlId is null || isActivated is null)
            return;
        string activatePhrase = isActivated.Value ? "Дезактивувати" : "Активувати";
        RowButton(activatePhrase, Q(ActivateUrl, urlId, !isActivated, sendNewMessage, @delegate, args));
    }

    [Action]
    protected async Task ActivateUrl(ObjectId urlId, bool isActivated, bool sendNewMessage, Delegate? @delegate = null, params object[] args)
    {
        _uriToVacanciesService.Activate(urlId, isActivated, CancelToken);
        string activatePhrase = "Посилання " + (isActivated ?  "активоване." : "дезактивоване.");
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
                methodInfo.Invoke(this, new List<object?>(args) { isActivated }.ToArray());
            }
        }
    }

}
