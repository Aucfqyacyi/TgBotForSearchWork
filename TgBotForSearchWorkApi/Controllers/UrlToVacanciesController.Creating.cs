using Deployf.Botf;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;
using System;
using Telegram.Bot.Types;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Services;


namespace TgBotForSearchWorkApi.Controllers;

public partial class UrlToVacanciesController
{
    [Action(Command.CreateUrl, CommandDescription.Empty)]
    public void CreateUrl()
    {
        ShowSiteNamesThenShowFilterCategories(0);
    }

    [Action]
    private void ShowSiteNamesThenShowFilterCategories(int urlIndex)
    {
        ShowSiteNames(siteType => Q(ShowFilterCategories, 0, urlIndex, siteType));
    }

    [Action]
    private void ShowFilterCategories(int page, int urlIndex, SiteType siteType)
    {
        Push($"Виберіть, потрібну категорію для фільтра.");
        var categories = _filterService.SiteTypeTo_CategoriesToFilters[siteType].Keys;
        Pager(categories, page, category => (category, Q(ShowFilters, 0, urlIndex, siteType, category)),
                                        Q(ShowFilterCategories, FirstPage, urlIndex, siteType), 1);
        RowButton(Back, Q(CreateUrl));
    }

    [Action]
    private async Task ShowFilters(int page, int urlIndex, SiteType siteType, string category)
    {
        Push($"Виберіть, потрібний фільтр.");
        Dictionary<int, Filter> indexsToFilters = _filterService.GetIndexInListToFilters(siteType, category);
        if(indexsToFilters.Count == 1 && indexsToFilters.First().Value.FilterType == FilterType.Text)
        {
            await AddFilterToUrlAsync(urlIndex, siteType, category, indexsToFilters.First().Key);
            return;
        }
        string format = Q(ShowFilters, FirstPage, urlIndex, siteType, category);
        Pager(indexsToFilters, page, indexsToFilters =>
                        (indexsToFilters.Value.Name, Q(AddFilterToUrlAsync, urlIndex, siteType, category, indexsToFilters.Key)),
                        format);
        if(urlIndex != 0)
            RowButton("Aктивувати нове посилання", Q(ActivateUrl, urlIndex, true));
        RowButton(Back, Q(ShowFilterCategories, 0, urlIndex, siteType));
    }

    [Action]
    private async Task AddFilterToUrlAsync(int urlIndex, SiteType siteType, string category, int filterIndex)
    {
        Filter filter = _filterService.SiteTypeTo_CategoriesToFilters[siteType][category][filterIndex];
        if (filter.FilterType == FilterType.CheckBox)
        {
            CreateOrUpdateUrlToVacanciesAsync(urlIndex, siteType, filter);
        }
        else
        {
            await State(new AddGetParametrValueState(urlIndex, filter, siteType));
            await Send("Введіть пошуковий запит.");
        }
            
    }

    [State]
    private void AddSearchFilterToUrl(AddGetParametrValueState state)
    {
        state.Filter.GetParametrValue = Context.GetSafeTextPayload()!;
        CreateOrUpdateUrlToVacanciesAsync(state.UrlIndex, state.SiteType, state.Filter);
    }

    [Action]
    private void CreateOrUpdateUrlToVacanciesAsync(int urlIndex, SiteType siteType, Filter filter)
    {
        int index = _userService.CreateOrUpdateUrlToVacancies(ChatId, urlIndex, siteType, filter, CancelToken);
        ShowFilterCategories(0, index, siteType);
    }
}
