using Parsers.Constants;
using Parsers.FilterParsers;
using Parsers.Models;
using System.Threading;

namespace TgBotForSearchWork.Services;


internal class FilterService
{
    private Dictionary<string, List<Filter>> _hostsToFilters = new();
    private object _lock = new object();   
    
    public async Task CollectFiltersAsync(CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(SiteTypesToUris.All, cancellationToken, CollectFilters);
    }

    private async ValueTask CollectFilters(KeyValuePair<SiteType, Uri> siteTypeToUri, CancellationToken cancellationToken)
    {
        IFilterParser filterParser = FilterParserFactory.CreateFilterParser(siteTypeToUri.Key);
        List<Filter> filters = await filterParser.ParseAsync(siteTypeToUri.Value, cancellationToken);
        lock (_lock)
        {
            _hostsToFilters.Add(siteTypeToUri.Value.Host, filters);
        }       
    }
}
