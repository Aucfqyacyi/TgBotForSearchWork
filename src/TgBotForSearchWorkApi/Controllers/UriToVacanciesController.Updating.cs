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
    public async Task AddFilter()
    {
        await ShowSitesThenShowUrisToVacancies(ShowFirstPageCategories);
    }

    [Action]
    private async Task ShowFirstPageCategories(ObjectId urlId, SiteType siteType)
    {
        ShowFilterCategories_Updating(0, siteType, urlId, await _uriToVacanciesRepository.IsActivatedAsync(urlId, CancelToken));
    }

    [Action]
    private void ShowFilterCategories_Updating(int page, SiteType siteType, ObjectId? urlId, bool isActivated)
    {
        Push(CommandRecommendations.All[siteType]);
        IEnumerable<FilterCategory> categories = _filterService.SiteTypeToCategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category.Name, Q(ShowFilters_Updating, 0, siteType, category.Id, urlId!, isActivated)),
                                        Q(ShowFilterCategories_Updating, _firstPage, siteType, urlId!, isActivated), 1);
        ActivateRowButton(urlId, isActivated, ShowFilterCategories_Updating, 0, siteType);
        RowButton(_back, Q(ShowUrisToVacancies, page, siteType, ShowFirstPageCategories));
    }

    /// <summary>
    /// Method ShowFilters with back button to the ShowFilterCategories_Updating.
    /// </summary>
    [Action]
    private void ShowFilters_Updating(int page, SiteType siteType, int categoryId, ObjectId urlId, bool isActivated)
    {
        ShowFilters(page, siteType, categoryId, ShowFilters_Updating, isUpdating: true, urlId, isActivated);
        RowButton(_back, Q(ShowFilterCategories_Updating, 0, siteType, urlId, isActivated));
    }

    [Action(Command.RemoveFilter, CommandDescription.RemoveFilter)]
    public async Task RemoveFilter()
    {
        await ShowSitesThenShowUrisToVacancies(ShowFirstPageCategoriesByGetParams);
    }

    [Action]
    private async Task ShowFirstPageCategoriesByGetParams(ObjectId urlId, SiteType siteType)
    {
        await ShowFilterCategoriesByGetParams(0, siteType, urlId, await _uriToVacanciesRepository.IsActivatedAsync(urlId, CancelToken));
    }

    [Action]
    private async Task ShowFilterCategoriesByGetParams(int page, SiteType siteType, ObjectId urlId, bool isActivated)
    {        
        UriToVacancies uriToVacancies = await _uriToVacanciesRepository.GetAsync(urlId, CancelToken);
        List<GetParameter> getParameters = uriToVacancies.GetParameters();
        IEnumerable<FilterCategory> categories = _filterService.GetFilterCategories(siteType, getParameters);
        Push("Виберіть категорію фільтра, яку бажаєте видалити з посилання.");
        Pager(categories, page, categories => (categories.Name,
                    Q(RemoveFilterFromUri, urlId!, siteType, categories.Id)),
                                        Q(ShowFilterCategoriesByGetParams, _firstPage, siteType, urlId!, isActivated), 1);
        ActivateRowButton(urlId, isActivated, ShowFilterCategoriesByGetParams, page, siteType);
        RowButton(_back, Q(ShowUrisToVacancies, 0, siteType, ShowFirstPageCategoriesByGetParams));
    }

    [Action]
    private async Task RemoveFilterFromUri(ObjectId urlId, SiteType siteType, int categoryId)
    {
        FilterCategory category =_filterService.SiteTypeToCategoriesToFilters[siteType].First(category => category.Key.Id == categoryId).Key;
        await _uriToVacanciesService.RemoveFilterAsync(urlId, category.GetParameterNames, CancelToken);
        await AnswerOkCallback();
        await ShowFirstPageCategoriesByGetParams(urlId, siteType);
    }
}
