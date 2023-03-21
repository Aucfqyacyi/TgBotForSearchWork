using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    private void ShowFilters(int page, SiteType siteType, int categoryId, Delegate nextInPagination,
                             bool isUpdating, ObjectId? urlId, bool isActivated)
    {
        Push($"Виберіть потрібний фільтр.");
        var idsToFilters = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId];
        Pager(idsToFilters, page, idToFilter =>
                  (idToFilter.Value.Name, Q(AddFilterToUrlAsync, urlId!, siteType, categoryId, idToFilter.Key, isUpdating)),
                        Q(nextInPagination, _firstPage, siteType, categoryId, urlId!, isActivated));
        ActivateRowButton(urlId, isActivated, nextInPagination, page, siteType, categoryId);
    }

    [Action]
    private async Task AddFilterToUrlAsync(ObjectId? urlId, SiteType siteType, int categoryId, int filterId, bool isUpdating)
    {
        Filter filter = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId][filterId];
        if (filter.FilterType == FilterType.CheckBox)
        {
            await CreateOrUpdateUriToVacanciesAsync(urlId, siteType, filter.GetParameter, isUpdating);
        }
        else
        {
            await State(new AddingTestFilterToUrlState(urlId, filter.GetParameter.Name, siteType, isUpdating));
            await Send("Введіть пошуковий запит.");
        }
    }

    [State]
    private async Task HandleAddingTestFilterToUrlAsync(AddingTestFilterToUrlState state)
    {
        await ClearState();
        string getParameterValue = Context.GetSafeTextPayload()!;
        GetParameter getParameter = new(state.GetParameterName, getParameterValue);
        await CreateOrUpdateUriToVacanciesAsync(state.UrlId, state.SiteType, getParameter, state.IsUpdating);
    }

    private async Task CreateOrUpdateUriToVacanciesAsync(ObjectId? urlId, SiteType siteType, GetParameter getParameter, bool isUpdating)
    {
        UriToVacancies? uriToVacancies = null;
        try
        {
            if (urlId is null)
                uriToVacancies = await _uriToVacanciesService.CreateAsync(ChatId, siteType, getParameter, CancelToken);
            else
                uriToVacancies = await _uriToVacanciesService.AddFilterAsync(urlId.Value, getParameter, CancelToken);
            await AnswerOkCallback();
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
            await Send("Посилання вже добавленно, оберіть інший фільтр.");
        }
        if (isUpdating)
            ShowFilterCategories_Updating(0, siteType, uriToVacancies?.Id, uriToVacancies?.IsActivated ?? false);
        else
            ShowFilterCategories_Creating(0, siteType, uriToVacancies?.Id, uriToVacancies?.IsActivated ?? false);
    }
}
