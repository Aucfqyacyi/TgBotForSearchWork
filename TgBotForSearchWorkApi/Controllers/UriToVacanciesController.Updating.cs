using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Controllers;


public partial class UriToVacanciesController
{
    [Action(Command.AddFilter, CommandDescription.Empty)]
    public void AddFilter()
    {
        ShowSiteNamesThenShowUrlsToVacancies(AddFilterToUriToVacancies);
    }

    [Action]
    private void AddFilterToUriToVacancies(ObjectId urlId, SiteType siteType)
    {
        ShowFilterCategories(0, urlId, siteType, _uriToVacanciesService.IsActivated(urlId, CancelToken));
        RowButton(Back, Q(GetUrlsToVacanciesAsync, 0, siteType, AddFilterToUriToVacancies));
    }

    [Action(Command.RemoveFilter, CommandDescription.Empty)]
    public void RemoveFilter()
    {
        ShowSiteNamesThenShowUrlsToVacancies(ShowFirstPageCategoriesByGetParams);
    }

    [Action]
    private void ShowFirstPageCategoriesByGetParams(ObjectId urlId, SiteType siteType)
    {
        ShowFilterCategoriesByGetParams(0, urlId, siteType, _uriToVacanciesService.IsActivated(urlId, CancelToken));
    }

    [Action]
    private void ShowFilterCategoriesByGetParams(int page, ObjectId urlId, SiteType siteType, bool isActivated)
    {
        Push("Виберіть категорію фільтра, яку бажаєте видалити з посилання.");
        UriToVacancies uriToVacancies = _uriToVacanciesService.Get(urlId, CancelToken);
        List<GetParameter> getParameters = uriToVacancies.GetParameters();
        List<FilterCategory> categories = _filterService.GetFilterCategories(siteType, getParameters);
        Pager(categories, 0, categories => (categories.Name, 
                    Q(RemoveFilterFromUri, urlId!, siteType, categories.Id, categories.GetParameterName)),
                                        Q(ShowFilterCategoriesByGetParams, FirstPage, urlId!, siteType, isActivated), 1);
        RowButton(Back, Q(GetUrlsToVacanciesAsync, 0, siteType, ShowFirstPageCategoriesByGetParams));
        ActivateRowButton(urlId, isActivated, false, ShowFilterCategoriesByGetParams, page, urlId!, siteType);
    }

    [Action]
    private async Task RemoveFilterFromUri(ObjectId urlId, SiteType siteType, int categoryId, string getParamName)
    {
        _uriToVacanciesService.Update(urlId, new(getParamName, string.Empty), false, CancelToken);
        await AnswerOkCallback();
        ShowFirstPageCategoriesByGetParams(urlId, siteType);
    }
}
