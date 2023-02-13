using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Models.States;


namespace TgBotForSearchWorkApi.Controllers;

public partial class UrlToVacanciesController
{
    [Action(Command.CreateUrl, CommandDescription.Empty)]
    public void CreateUrl()
    {
        GetSiteNamesThenGetFilterCategories(null);
    }

    [Action]
    private void GetSiteNamesThenGetFilterCategories(ObjectId? urlId)
    {
        GetSiteNames(siteType => Q(GetFilterCategories, 0, urlId, siteType));
    }

    [Action]
    private void GetFilterCategories(int page, ObjectId? urlId, SiteType siteType)
    {
        Push($"Виберіть, потрібну категорію для фільтра.");
        var categories = _filterService.SiteTypeToCategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category.Name, Q(GetFilters, 0, urlId, siteType, category.Id)),
                                        Q(GetFilterCategories, FirstPage, urlId, siteType), 1);
        //AddExtraButtonsToList(urlIndex, siteType);
        RowButton(Back, Q(GetSiteNamesThenGetFilterCategories, urlId));
    }

    [Action]
    private void GetFilters(int page, ObjectId? urlId, SiteType siteType, int categoryId)
    {
        Push($"Виберіть, потрібний фільтр.");
        var idsToFilters = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId];
        string format = Q(GetFilters, FirstPage, urlId, siteType, categoryId);
        Pager(idsToFilters, page, idToFilter =>
                        (idToFilter.Value.Name, Q(AddFilterToUrlAsync, urlId, siteType, categoryId, idToFilter.Key)),
                        format);
        //AddExtraButtonsToList(urlId, siteType);
        RowButton(Back, Q(GetFilterCategories, 0, urlId, siteType));
    }

    private void AddExtraButtonsToList(int urlIndex, SiteType siteType)
    {
        if (urlIndex != 0)
            RowButton("Aктивувати нове посилання", Q(ActivateUrl, urlIndex, true));

    }

    [Action]
    private async Task AddFilterToUrlAsync(ObjectId? urlId, SiteType siteType, int categoryId, int filterId)
    {
        Filter filter = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId][filterId];
        if (filter.FilterType == FilterType.CheckBox)
        {
            await CreateOrUpdateUrlToVacanciesAsync(urlId, siteType, filter.GetParametr);
        }
        else
        {
            await State(new AddingSearchFilterToUrlState(urlId, filter.GetParametr.Name, siteType));
            await Send("Введіть пошуковий запит.");
        }
            
    }

    [State]
    private async Task AddSearchFilterToUrlAsync(AddingSearchFilterToUrlState state)
    {        
        string getParametrValue = Context.GetSafeTextPayload()!;
        GetParametr getParametr = new(state.GetParametrName, getParametrValue);
        await CreateOrUpdateUrlToVacanciesAsync(state.UrlId, state.SiteType, getParametr);
    }

    private async Task CreateOrUpdateUrlToVacanciesAsync(ObjectId? urlId, SiteType siteType, GetParametr getParametr)
    {
        UrlToVacancies? urlToVacancies = null;
        if (urlId is null)
            urlToVacancies = _urlToVacanciesService.Create(ChatId, siteType, getParametr, CancelToken);
        else
            urlToVacancies = _urlToVacanciesService.Update(ChatId, urlId.Value, getParametr, CancelToken);
        GetFilterCategories(0, urlToVacancies.Id, siteType);
        await AnswerCallback("Ok");
    }
}
