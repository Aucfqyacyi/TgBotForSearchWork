using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using System.Reflection;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Services;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController : BotController
{
    private const string _back = "Назад";
    private const string _firstPage = "{0}";

    private readonly UriToVacanciesService _uriToVacanciesService;
    private readonly FilterService _filterService;
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;

    public UriToVacanciesController(FilterService filterService,
                                    UriToVacanciesService uriToVacanciesService,
                                    UriToVacanciesRepository uriToVacanciesRepository)
    {
        _filterService = filterService;
        _uriToVacanciesService = uriToVacanciesService;
        _uriToVacanciesRepository = uriToVacanciesRepository;
    }

    private void ShowSites(Func<SiteType, string> q, IEnumerable<SiteType>? siteTypes = null)
    {
        Push("Виберіть сайт.");
        foreach (var siteType in siteTypes ?? Enum.GetValues<SiteType>())
        {
            RowButton(siteType.ToString(), q(siteType));
        }
    }

    private void ActivateRowButton(ObjectId? urlId, bool? isActivated, Delegate? @delegate = null, params object[] args)
    {
        if (urlId is null || isActivated is null)
            return;
        string activatePhrase = isActivated.Value ? "Дезактивувати" : "Активувати";
        RowButton(activatePhrase, Q(ActivateUrl, urlId, !isActivated, @delegate!, args));
    }

    [Action]
    private async Task ActivateUrl(ObjectId urlId, bool isActivated, Delegate? @delegate = null, params object[] args)
    {
        await _uriToVacanciesRepository.ActivateAsync(urlId, isActivated, CancelToken);
        if (@delegate is null)
        {
            await AnswerCallback();
            await Send("Посилання " + (isActivated ? "активоване." : "дезактивоване."));
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
