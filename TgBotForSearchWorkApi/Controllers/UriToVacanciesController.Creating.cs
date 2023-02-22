using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    [Action(Command.CreateUrl, CommandDescription.Empty)]
    public void CreateUrl()
    {
        GetSiteNamesThenShowFilterCategories(null, false);
    }

    [Action]
    private void GetSiteNamesThenShowFilterCategories(ObjectId? urlId, bool isActivated)
    {
        ShowSiteNames(siteType => Q(ShowFilterCategoriesWithBackButton, 0, urlId!, siteType, isActivated));
    }

    [Action]
    private void ShowFilterCategoriesWithBackButton(int page, ObjectId? urlId, SiteType siteType, bool isActivated)
    {
        ShowFilterCategories(page, urlId, siteType, isActivated);
        if (urlId is null)
            RowButton(Back, Q(GetSiteNamesThenShowFilterCategories, urlId!, isActivated));
    }

    [Action]
    private void ShowFilterCategories(int page, ObjectId? urlId, SiteType siteType, bool isActivated)
    {
        Push($"Виберіть, потрібну категорію для фільтра.");
        IEnumerable<FilterCategory> categories = _filterService.SiteTypeToCategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category.Name, Q(ShowFilters, 0, urlId!, siteType, category.Id, isActivated)),
                                        Q(ShowFilterCategories, FirstPage, urlId!, siteType, isActivated), 1);
        ActivateRowButton(urlId, isActivated, false, ShowFilterCategories, page, urlId!, siteType);        
    }

    [Action]
    private void ShowFilters(int page, ObjectId? urlId, SiteType siteType, int categoryId, bool isActivated)
    {
        Push($"Виберіть, потрібний фільтр.");
        var idsToFilters = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId];
        string format = Q(ShowFilters, FirstPage, urlId!, siteType, categoryId, isActivated);
        Pager(idsToFilters, page, idToFilter =>
                        (idToFilter.Value.Name, Q(AddFilterToUrlAsync, urlId!, siteType, categoryId, idToFilter.Key)),
                        format);
        ActivateRowButton(urlId, isActivated, false, ShowFilters, page, urlId!, siteType, categoryId);
        RowButton(Back, Q(ShowFilterCategoriesWithBackButton, 0, urlId!, siteType, isActivated));
    }

    [Action]
    private async Task AddFilterToUrlAsync(ObjectId? urlId, SiteType siteType, int categoryId, int filterId)
    {
        Filter filter = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId][filterId];
        if (filter.FilterType == FilterType.CheckBox)
        {
            await CreateOrUpdateUriToVacanciesAsync(urlId, siteType, filter.GetParameter);
        }
        else
        {
            await State(new AddingSearchFilterToUrlState(urlId, filter.GetParameter.Name, siteType));
            await Send("Введіть пошуковий запит.");
        }
    }

    [State]
    private async Task HandleAddingSearchFilterToUrlAsync(AddingSearchFilterToUrlState state)
    {
        string getParameterValue = Context.GetSafeTextPayload()!;
        GetParameter getParameter = new(state.GetParameterName, getParameterValue);
        await CreateOrUpdateUriToVacanciesAsync(state.UrlId, state.SiteType, getParameter);
    }

    private async Task CreateOrUpdateUriToVacanciesAsync(ObjectId? urlId, SiteType siteType, GetParameter getParameter)
    {
        UriToVacancies? uriToVacancies = null;        
        try
        {
            if (urlId is null)
                uriToVacancies = _uriToVacanciesService.Create(ChatId, siteType, getParameter, CancelToken);
            else
                uriToVacancies = _uriToVacanciesService.Update(urlId.Value, getParameter, true, CancelToken);
            await AnswerOkCallback();
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
            await Send("Посилання вже добавленно, оберіть інший фільтр.");
        }
        ShowFilterCategories(0, uriToVacancies?.Id ?? urlId, siteType, uriToVacancies?.IsActivated ?? false);       
    }
}
