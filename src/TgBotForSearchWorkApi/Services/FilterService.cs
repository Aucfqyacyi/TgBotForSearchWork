﻿using Parsers.Constants;
using Parsers.FilterParsers;
using Parsers.Models;

namespace TgBotForSearchWorkApi.Services;


public class FilterService
{
    private SortedDictionary<SiteType, IReadOnlyDictionary<FilterCategory, IReadOnlyDictionary<int, Filter>>> _siteTypeToCategoriesToFilters = new();
    public IReadOnlyDictionary<SiteType, IReadOnlyDictionary<FilterCategory, IReadOnlyDictionary<int, Filter>>> SiteTypeToCategoriesToFilters
    {
        get => _siteTypeToCategoriesToFilters;
    }

    public async ValueTask CollectFiltersAsync(CancellationToken cancellationToken = default)
    {
        await Parallel.ForEachAsync(SiteTypesToUris.All, cancellationToken, CollectFiltersAsync);
    }

    private async ValueTask CollectFiltersAsync(KeyValuePair<SiteType, Uri> siteTypeToUri, CancellationToken cancellationToken)
    {
        IFilterParser filterParser = FilterParserFactory.Create(siteTypeToUri.Key);
        List<Filter> filters = await filterParser.ParseAsync(siteTypeToUri.Value, cancellationToken);
        SortedDictionary<FilterCategory, IReadOnlyDictionary<int, Filter>> categoriesToFilters = new();
        SortedDictionary<int, Filter>? idsToFilters = null;
        foreach (Filter filter in filters)
        {
            if (categoriesToFilters.ContainsKey(filter.Category))
            {
                idsToFilters!.Add(filter.Id, filter);
            }
            else
            {
                idsToFilters = new() { { filter.Id, filter } };
                categoriesToFilters.Add(filter.Category, idsToFilters);
            }
        }
        lock (this)
            _siteTypeToCategoriesToFilters.Add(siteTypeToUri.Key, categoriesToFilters);
    }

    public List<FilterCategory> GetFilterCategories(SiteType siteType, List<GetParameter> getParameters)
    {
        var allCategories = _siteTypeToCategoriesToFilters[siteType].Keys;
        return allCategories.Aggregate(new List<FilterCategory>(), (categories, category) =>
        {
            GetParameter? getParameter = getParameters.FirstOrDefault(getParam => getParam.Name == category.GetParameterName);
            if (getParameter is not null)
                categories.Add(category);
            return categories;
        });
    }
}