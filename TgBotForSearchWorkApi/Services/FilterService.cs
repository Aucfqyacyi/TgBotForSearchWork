using Parsers.Constants;
using Parsers.FilterParsers;
using Parsers.Models;

namespace TgBotForSearchWorkApi.Services;


public class FilterService
{
    private SortedDictionary<SiteType, SortedDictionary<string, List<Filter>>> _siteTypeTo_CategoriesToFilters = new();
    public IReadOnlyDictionary<SiteType, SortedDictionary<string, List<Filter>>> SiteTypeTo_CategoriesToFilters
    {
        get => _siteTypeTo_CategoriesToFilters;
    }

    public Task CollectFiltersAsync(CancellationToken cancellationToken = default)
    {
        return Parallel.ForEachAsync(SiteTypesToUris.All, cancellationToken, CollectFiltersAsync);
    }

    private async ValueTask CollectFiltersAsync(KeyValuePair<SiteType, Uri> siteTypeToUri, CancellationToken cancellationToken)
    {
        IFilterParser filterParser = FilterParserFactory.CreateFilterParser(siteTypeToUri.Key);
        List<Filter> filters = await filterParser.ParseAsync(siteTypeToUri.Value, cancellationToken);
        SortedDictionary<string, List<Filter>> categoriesToFilters = new();
        foreach (Filter filter in filters)
        {
            List<Filter>? filteredFilters = categoriesToFilters.GetValueOrDefault(filter.CategoryName);
            if (filteredFilters is null)
            {
                filteredFilters= new List<Filter>();
                categoriesToFilters.Add(filter.CategoryName, filteredFilters);
            }
            filteredFilters.Add(filter);
        }
        lock (this)
            _siteTypeTo_CategoriesToFilters.Add(siteTypeToUri.Key, categoriesToFilters);
    }

    public Dictionary<int, Filter> GetIndexInListToFilters(SiteType siteType, string category) 
    {
        int i = 0;
        return SiteTypeTo_CategoriesToFilters[siteType][category].Aggregate(new Dictionary<int, Filter>(), (indexsToFilters, filter) =>
        {
            indexsToFilters.Add(i++, filter);
            return indexsToFilters;
        });
    }
}
