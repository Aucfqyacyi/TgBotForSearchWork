using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    [Action(Command.CreateUrl, CommandDescription.Empty)]
    public void CreateUrl()
    {
        GetSiteNamesThenGetFilterCategories(null, false);
    }

    [Action]
    private void GetSiteNamesThenGetFilterCategories(ObjectId? urlId, bool isActivate)
    {
        GetSiteNames(siteType => Q(GetFilterCategories, 0, urlId, siteType, isActivate));
    }

    [Action]
    private void GetFilterCategories(int page, ObjectId? urlId, SiteType siteType, bool isActivate)
    {
        Push($"Виберіть, потрібну категорію для фільтра.");
        var categories = _filterService.SiteTypeToCategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category.Name, Q(GetFilters, 0, urlId, siteType, category.Id, isActivate)),
                                        Q(GetFilterCategories, FirstPage, urlId, siteType, isActivate), 1);
        ActivateRowButton(urlId, isActivate, false, GetFilterCategories, page, urlId, siteType);
        if (urlId is null)
            RowButton(Back, Q(GetSiteNamesThenGetFilterCategories, urlId, isActivate));
    }

    [Action]
    private void GetFilters(int page, ObjectId? urlId, SiteType siteType, int categoryId, bool isActivate)
    {
        Push($"Виберіть, потрібний фільтр.");
        var idsToFilters = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId];
        string format = Q(GetFilters, FirstPage, urlId, siteType, categoryId, isActivate);
        Pager(idsToFilters, page, idToFilter =>
                        (idToFilter.Value.Name, Q(AddFilterToUrlAsync, urlId, siteType, categoryId, idToFilter.Key)),
                        format);
        ActivateRowButton(urlId, isActivate, false, GetFilters, page, urlId, siteType, categoryId);
        RowButton(Back, Q(GetFilterCategories, 0, urlId, siteType, isActivate));
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
        if (urlId is null)
            uriToVacancies = _uriToVacanciesService.Create(ChatId, siteType, getParametr, CancelToken);
        else
            uriToVacancies = _uriToVacanciesService.Update(urlId.Value, getParametr, CancelToken);
        GetFilterCategories(0, uriToVacancies.Id, siteType, uriToVacancies.IsActivate);
        await AnswerOkCallback();
    }
}
