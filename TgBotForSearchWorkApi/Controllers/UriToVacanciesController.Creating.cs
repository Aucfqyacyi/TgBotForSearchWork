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
        GetSiteNamesThenGetFilterCategories(null, false);
    }

    [Action]
    private void GetSiteNamesThenGetFilterCategories(ObjectId? urlId, bool isActivated)
    {
        GetSiteNames(siteType => Q(GetFilterCategories, 0, urlId, siteType, isActivated));
    }

    [Action]
    private void GetFilterCategories(int page, ObjectId? urlId, SiteType siteType, bool isActivated)
    {
        Push($"Виберіть, потрібну категорію для фільтра.");
        var categories = _filterService.SiteTypeToCategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category.Name, Q(GetFilters, 0, urlId, siteType, category.Id, isActivated)),
                                        Q(GetFilterCategories, FirstPage, urlId, siteType, isActivated), 1);
        ActivateRowButton(urlId, isActivated, false, GetFilterCategories, page, urlId, siteType);
        if (urlId is null)
            RowButton(Back, Q(GetSiteNamesThenGetFilterCategories, urlId, isActivated));
    }

    [Action]
    private void GetFilters(int page, ObjectId? urlId, SiteType siteType, int categoryId, bool isActivated)
    {
        Push($"Виберіть, потрібний фільтр.");
        var idsToFilters = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId];
        string format = Q(GetFilters, FirstPage, urlId, siteType, categoryId, isActivated);
        Pager(idsToFilters, page, idToFilter =>
                        (idToFilter.Value.Name, Q(AddFilterToUrlAsync, urlId, siteType, categoryId, idToFilter.Key)),
                        format);
        ActivateRowButton(urlId, isActivated, false, GetFilters, page, urlId, siteType, categoryId);
        RowButton(Back, Q(GetFilterCategories, 0, urlId, siteType, isActivated));
    }

    [Action]
    private async Task AddFilterToUrlAsync(ObjectId? urlId, SiteType siteType, int categoryId, int filterId)
    {
        Filter filter = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId][filterId];
        if (filter.FilterType == FilterType.CheckBox)
        {
            await CreateOrUpdateUriToVacanciesAsync(urlId, siteType, filter.GetParametr);
        }
        else
        {
            await State(new AddingSearchFilterToUrlState(urlId, filter.GetParametr.Name, siteType));
            await Send("Введіть пошуковий запит.");
        }
    }

    [State]
    private async Task HandleAddingSearchFilterToUrlAsync(AddingSearchFilterToUrlState state)
    {
        string getParametrValue = Context.GetSafeTextPayload()!;
        GetParametr getParametr = new(state.GetParametrName, getParametrValue);
        await CreateOrUpdateUriToVacanciesAsync(state.UrlId, state.SiteType, getParametr);
    }

    private async Task CreateOrUpdateUriToVacanciesAsync(ObjectId? urlId, SiteType siteType, GetParametr getParametr)
    {
        UriToVacancies? uriToVacancies = null;        
        try
        {
            if (urlId is null)
                uriToVacancies = _uriToVacanciesService.Create(ChatId, siteType, getParametr, CancelToken);
            else
                uriToVacancies = _uriToVacanciesService.Update(urlId.Value, getParametr, CancelToken);
            await AnswerOkCallback();
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
            await Send("Посилання вже добавленно, оберіть інший фільтр.");
        }
        GetFilterCategories(0, uriToVacancies?.Id ?? urlId, siteType, uriToVacancies?.IsActivated ?? false);       
    }
}
