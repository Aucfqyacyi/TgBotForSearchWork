using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using System.Reflection;
using TgBotForSearchWorkApi.Services;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController : BotController
{
    protected const string Back = "Назад";
    protected const string FirstPage = "{0}";

    protected readonly UriToVacanciesService _uriToVacanciesService;
    protected readonly FilterService _filterService;

    public UriToVacanciesController(FilterService filterService, UriToVacanciesService uriToVacanciesService)
    {
        _filterService = filterService;
        _uriToVacanciesService = uriToVacanciesService;
    }

    protected void ShowSites(Func<SiteType, string> q)
    {
        Push("Виберіть сайт.");
        foreach (var siteType in Enum.GetValues<SiteType>())
        {
            RowButton(siteType.ToString(), q(siteType));
        }
    }

    protected void ActivateRowButton(ObjectId? urlId, bool? isActivated, Delegate? @delegate = null, params object[] args)
    {
        if (urlId is null || isActivated is null)
            return;
        string activatePhrase = isActivated.Value ? "Дезактивувати" : "Активувати";
        RowButton(activatePhrase, Q(ActivateUrl, urlId, !isActivated, @delegate!, args));
    }

    [Action]
    protected async Task ActivateUrl(ObjectId urlId, bool isActivated, Delegate? @delegate = null, params object[] args)
    {
        await _uriToVacanciesService.ActivateAsync(urlId, isActivated, CancelToken);
        string activatePhrase = "Посилання " + (isActivated ? "активоване." : "дезактивоване.");
        if (@delegate is null)
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
                methodInfo.Invoke(this, new List<object?>(args) { urlId, isActivated }.ToArray());
            }
        }
    }

}
