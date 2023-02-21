using Parsers.Constants;
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


    public async Task CollectFiltersAsync(CancellationToken cancellationToken = default)
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
}
