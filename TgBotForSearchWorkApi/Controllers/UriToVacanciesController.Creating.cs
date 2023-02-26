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
        ShowSitesThenShowFilterCategories(null, false);
    }

    [Action]
    private void ShowSitesThenShowFilterCategories(ObjectId? urlId, bool isActivated)
    {
        ShowSites(siteType => Q(ShowFilterCategories_Creating, 0, siteType, urlId!, isActivated));
    }

    /// <summary>
    /// Method ShowFilterCategories with back button to ShowSitesThenShowFilterCategories.
    /// </summary>
    [Action]
    private void ShowFilterCategories_Creating(int page, SiteType siteType, ObjectId? urlId, bool isActivated)
    {
        ShowFilterCategories(page, siteType, ShowFilters_Creating, urlId, isActivated);
        if (urlId is null)
            RowButton(Back, Q(ShowSitesThenShowFilterCategories, urlId!, isActivated));
    }

    /// <summary>
    /// Method ShowFilters with back button to ShowFilterCategories_Creating.
    /// </summary>
    [Action]
    private void ShowFilters_Creating(int page, SiteType siteType, int categoryId, ObjectId? urlId, bool isActivated)
    {
        ShowFilters(page, siteType, categoryId, ShowFilters_Creating, urlId, isActivated);
        RowButton(Back, Q(ShowFilterCategories_Creating, 0, siteType, urlId!, isActivated));
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
        await ClearState();
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
        ShowFilterCategories(0, siteType, ShowFilters, uriToVacancies?.Id ?? urlId, uriToVacancies?.IsActivated ?? false);       
    }
}
