using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
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

    protected void ShowSites(Func<SiteType, string> q)
    {
        Push("Виберіть, сайт.");
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
        _uriToVacanciesService.Activate(urlId, isActivated, CancelToken);
        string activatePhrase = "Посилання " + (isActivated ?  "активоване." : "дезактивоване.");
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

    [Action]
    private void ShowFilterCategories(int page, SiteType siteType, Delegate next, ObjectId? urlId, bool isActivated)
    {
        Push($"Виберіть, потрібну категорію для фільтра.");
        IEnumerable<FilterCategory> categories = _filterService.SiteTypeToCategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category.Name, Q(next, 0, siteType, category.Id, urlId!, isActivated)),
                                        Q(ShowFilterCategories, FirstPage, siteType, next, urlId!, isActivated), 1);
        ActivateRowButton(urlId, isActivated, ShowFilterCategories, page, siteType, next);
    }

    [Action]
    private void ShowFilters(int page, SiteType siteType, int categoryId, Delegate nextInPagination, ObjectId? urlId, bool isActivated)
    {
        Push($"Виберіть, потрібний фільтр.");
        var idsToFilters = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId];
        string format = Q(nextInPagination, FirstPage, siteType, categoryId, urlId!, isActivated);
        Pager(idsToFilters, page, idToFilter =>
                        (idToFilter.Value.Name, Q(AddFilterToUrlAsync, urlId!, siteType, categoryId, idToFilter.Key)),
                        format);
        ActivateRowButton(urlId, isActivated, ShowFilters, page, siteType, categoryId);
    }

}
