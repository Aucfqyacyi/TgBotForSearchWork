﻿using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{

    [Action]
    private void ShowFilters(int page, SiteType siteType, int categoryId, Delegate nextInPagination,
                             bool canBackToMainPage, ObjectId? urlId, bool isActivated)
    {
        Push($"Виберіть потрібний фільтр.");
        var idsToFilters = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId];
        Pager(idsToFilters, page, idToFilter =>
                  (idToFilter.Value.Name, Q(AddFilterToUrlAsync, urlId!, siteType, categoryId, idToFilter.Key, canBackToMainPage)),
                        Q(nextInPagination, FirstPage, siteType, categoryId, urlId!, isActivated));
        ActivateRowButton(urlId, isActivated, nextInPagination, page, siteType, categoryId);
    }

    [Action]
    private async Task AddFilterToUrlAsync(ObjectId? urlId, SiteType siteType, int categoryId, int filterId, bool canBackToMainPage)
    {
        Filter filter = _filterService.SiteTypeToCategoriesToFilters[siteType][categoryId][filterId];
        if (filter.FilterType == FilterType.CheckBox)
        {
            await CreateOrUpdateUriToVacanciesAsync(urlId, siteType, filter.GetParameter, canBackToMainPage);
        }
        else
        {
            await State(new AddingSearchFilterToUrlState(urlId, filter.GetParameter.Name, siteType, canBackToMainPage));
            await Send("Введіть пошуковий запит.");
        }
    }

    [State]
    private async Task HandleAddingSearchFilterToUrlAsync(AddingSearchFilterToUrlState state)
    {
        await ClearState();
        string getParameterValue = Context.GetSafeTextPayload()!;
        GetParameter getParameter = new(state.GetParameterName, getParameterValue);
        await CreateOrUpdateUriToVacanciesAsync(state.UrlId, state.SiteType, getParameter, state.CanBackToMainPage);
    }

    private async Task CreateOrUpdateUriToVacanciesAsync(ObjectId? urlId, SiteType siteType, GetParameter getParameter, bool canBackToMainPage)
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
        if (canBackToMainPage)
            ShowFilterCategories_Updating(0, siteType, uriToVacancies?.Id, uriToVacancies?.IsActivated ?? false);
        else
            ShowFilterCategories_Creating(0, siteType, uriToVacancies?.Id, uriToVacancies?.IsActivated ?? false);
    }
}