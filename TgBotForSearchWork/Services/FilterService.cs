using Parsers.Constants;
using Parsers.FilterParsers;
using Parsers.Models;

namespace TgBotForSearchWork.Services;


internal class FilterService
{
    public IReadOnlyDictionary<SiteType, List<Filter>> SiteTypeToFilters { get => _siteTypeToFilters; }
    private SortedDictionary<SiteType, List<Filter>> _siteTypeToFilters = new();
    private object _lock = new object();

    public Task CollectFiltersAsync(CancellationToken cancellationToken = default)
    {
        return Parallel.ForEachAsync(SiteTypesToUris.All, cancellationToken, CollectFiltersAsync);
    }

    private async ValueTask CollectFiltersAsync(KeyValuePair<SiteType, Uri> siteTypeToUri, CancellationToken cancellationToken)
    {
        IFilterParser filterParser = FilterParserFactory.CreateFilterParser(siteTypeToUri.Key);
        List<Filter> filters = await filterParser.ParseAsync(siteTypeToUri.Value, cancellationToken);
        lock (_lock)
        {
            _siteTypeToFilters.Add(siteTypeToUri.Key, filters);
        }
    }

    public List<string> GetFilterCategories(string siteType)
    {
        return GetFilterCategories(Enum.Parse<SiteType>(siteType));
    }

    public List<string> GetFilterCategories(SiteType siteType)
    {
        return _siteTypeToFilters[siteType].Aggregate(new List<string>(), (categoryNames, filters) =>
        {
            if(!categoryNames.Contains(filters.CategoryName))
                categoryNames.Add(filters.CategoryName);
            return categoryNames;
        });
    }

}
