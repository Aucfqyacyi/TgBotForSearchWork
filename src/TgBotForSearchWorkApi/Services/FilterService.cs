using AutoDIInjector.Attributes;
using Parsers.Constants;
using Parsers.FilterParsers;
using Parsers.Models;
using Parsers.ParserFactories;
using TgBotForSearchWorkApi.Extensions;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class FilterService
{
    private readonly FilterParserFactory _filterParserFactory;
    private readonly SemaphoreSlim _semaphoreSlim = new (initialCount:1);

    private readonly SortedDictionary<SiteType, IReadOnlyDictionary<FilterCategory, IReadOnlyDictionary<int, Filter>>> _siteTypeToCategoriesToFilters = new();
    public IReadOnlyDictionary<SiteType, IReadOnlyDictionary<FilterCategory, IReadOnlyDictionary<int, Filter>>> SiteTypeToCategoriesToFilters
    {
        get => _siteTypeToCategoriesToFilters;
    }

    public FilterService(FilterParserFactory filterParserFactory)
    {
        _filterParserFactory = filterParserFactory;
    }

    public async ValueTask CollectFiltersAsync(CancellationToken cancellationToken = default)
    {
        await Parallel.ForEachAsync(SiteTypesToUris.All, cancellationToken, CollectFiltersAsync);
    }

    private async ValueTask CollectFiltersAsync(KeyValuePair<SiteType, Uri> siteTypeToUri, CancellationToken cancellationToken)
    {
        IFilterParser filterParser = _filterParserFactory.GetOrCreate(siteTypeToUri.Key);
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
                idsToFilters = new SortedDictionary<int, Filter>() { { filter.Id, filter } };
                categoriesToFilters.Add(filter.Category, idsToFilters);
            }
        }
        await _semaphoreSlim.WaitAsync(cancellationToken);
        _siteTypeToCategoriesToFilters.Add(siteTypeToUri.Key, categoriesToFilters);
        _semaphoreSlim.Release();
    }

    public IEnumerable<FilterCategory> GetFilterCategories(SiteType siteType, List<GetParameter> getParameters)
    {
        IEnumerable<FilterCategory> allCategories = _siteTypeToCategoriesToFilters[siteType].Keys;
        IEnumerable<string> getParameterNames = getParameters.Select(getParameter => getParameter.Name);
        return allCategories.Where(category => category.GetParameterNames.Contains(getParameterNames));
    }
}
