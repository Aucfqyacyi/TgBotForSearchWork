using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using TgBotForSearchWorkApi.Constants;
namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    [Action(Command.CreateUrl, CommandDescription.CreateUrl)]
    public void CreateUrl()
    {
        ShowSitesThenShowFilterCategories(urlId: null, isActivated: false);
    }

    [Action]
    private void ShowSitesThenShowFilterCategories(ObjectId? urlId, bool isActivated)
    {
        ShowSites(siteType => Q(ShowFilterCategories_Creating, 0, siteType, urlId!, isActivated));
    }

    [Action]
    private void ShowFilterCategories_Creating(int page, SiteType siteType, ObjectId? urlId, bool isActivated)
    {
        Push(CommandRecommendations.All[siteType]);
        IEnumerable<FilterCategory> categories = _filterService.SiteTypeToCategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category.Name, Q(ShowFilters_Creating, 0, siteType, category.Id, urlId!, isActivated)),
                                        Q(ShowFilterCategories_Creating, _firstPage, siteType, urlId!, isActivated), 1);
        ActivateRowButton(urlId, isActivated, ShowFilterCategories_Creating, page, siteType);
        RowButton(_back, Q(ShowSitesThenShowFilterCategories, new ObjectId?(), isActivated));
    }

    /// <summary>
    /// Method ShowFilters with back button to ShowFilterCategories_Creating.
    /// </summary>
    [Action]
    private void ShowFilters_Creating(int page, SiteType siteType, int categoryId, ObjectId? urlId, bool isActivated)
    {
        ShowFilters(page, siteType, categoryId, ShowFilters_Creating, isUpdating: false, urlId, isActivated);
        RowButton(_back, Q(ShowFilterCategories_Creating, 0, siteType, urlId!, isActivated));
    }
}
