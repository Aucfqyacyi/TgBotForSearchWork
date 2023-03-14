using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Controllers;


public partial class UriToVacanciesController
{
    [Action(Command.AddFilter, CommandDescription.AddFilter)]
    public void AddFilter()
    {
        ShowSitesThenShowUrisToVacancies(ShowFirstPageCategories);
    }

    [Action]
    private async Task ShowFirstPageCategories(ObjectId urlId, SiteType siteType)
    {
        ShowFilterCategories_Updating(0, siteType, urlId, await _uriToVacanciesService.IsActivatedAsync(urlId, CancelToken));
    }

    /// <summary>
    /// Method ShowFilterCategories with back button to ShowSitesThenShowUrisToVacancies.
    /// </summary>
    [Action]
    private void ShowFilterCategories_Updating(int page, SiteType siteType, ObjectId? urlId, bool isActivated)
    {
        Push($"Виберіть потрібну категорію для фільтра.");
        IEnumerable<FilterCategory> categories = _filterService.SiteTypeToCategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category.Name, Q(ShowFilters_Updating, 0, siteType, category.Id, urlId!, isActivated)),
                                        Q(ShowFilterCategories_Updating, FirstPage, siteType, urlId!, isActivated), 1);
        ActivateRowButton(urlId, isActivated, ShowFilterCategories_Updating, 0, siteType);
        RowButton(Back, Q(ShowUrisToVacancies, page, siteType, ShowFirstPageCategories));
    }

    /// <summary>
    /// Method ShowFilters with back button to the ShowFilterCategories_Updating.
    /// </summary>
    [Action]
    private void ShowFilters_Updating(int page, SiteType siteType, int categoryId, ObjectId urlId, bool isActivated)
    {
        ShowFilters(page, siteType, categoryId, ShowFilters_Updating, isUpdating:true, urlId, isActivated);
        RowButton(Back, Q(ShowFilterCategories_Updating, 0, siteType, urlId, isActivated));
    }

    [Action(Command.RemoveFilter, CommandDescription.RemoveFilter)]
    public void RemoveFilter()
    {
        ShowSitesThenShowUrisToVacancies(ShowFirstPageCategoriesByGetParams);
    }

    [Action]
    private async Task ShowFirstPageCategoriesByGetParams(ObjectId urlId, SiteType siteType)
    {
        await ShowFilterCategoriesByGetParams(0, siteType, urlId, await _uriToVacanciesService.IsActivatedAsync(urlId, CancelToken));
    }

    [Action]
    private async Task ShowFilterCategoriesByGetParams(int page, SiteType siteType, ObjectId urlId, bool isActivated)
    {
        Push("Виберіть категорію фільтра, яку бажаєте видалити з посилання.");
        UriToVacancies uriToVacancies = await _uriToVacanciesService.GetAsync(urlId, CancelToken);
        List<GetParameter> getParameters = uriToVacancies.GetParameters();
        List<FilterCategory> categories = _filterService.GetFilterCategories(siteType, getParameters);
        Pager(categories, page, categories => (categories.Name,
                    Q(RemoveFilterFromUri, urlId!, siteType, categories.Id, categories.GetParameterName)),
                                        Q(ShowFilterCategoriesByGetParams, FirstPage, siteType, urlId!, isActivated), 1);
        ActivateRowButton(urlId, isActivated, ShowFilterCategoriesByGetParams, page, siteType);
        RowButton(Back, Q(ShowUrisToVacancies, 0, siteType, ShowFirstPageCategoriesByGetParams));
    }

    [Action]
    private async Task RemoveFilterFromUri(ObjectId urlId, SiteType siteType, int categoryId, string getParamName)
    {
        await _uriToVacanciesService.RemoveFilterAsync(urlId, new(getParamName, string.Empty), CancelToken);
        await AnswerOkCallback();
        await ShowFirstPageCategoriesByGetParams(urlId, siteType);
    }
}
