using Parsers.Constants;
using Parsers.FilterParsers.Parsers;

namespace Parsers.FilterParsers;

public class FilterParserFactory
{
    private readonly static Dictionary<SiteType, IFilterParser> _cache = new();
    private readonly static object _lock = new object();

    public static IFilterParser Create(SiteType sitetype)
    {
        switch (sitetype)
        {
            case SiteType.Dou:
                return Create<DouFilterParser>(sitetype);
            case SiteType.Djinni:
                return Create<DjinniFilterParser>(sitetype);
            case SiteType.WorkUa:
                return Create<WorkUaFilterParser>(sitetype);
        }
        throw new Exception($"Parser with sitetype({sitetype}) was not found.");
    }

    private static IFilterParser Create<TParser>(SiteType site)
                                                where TParser : class, IFilterParser, new()
    {
        IFilterParser? filterParser = null;
        lock (_lock)
        {
            filterParser = _cache.GetValueOrDefault(site);
            if (filterParser is null)
            {
                filterParser = new TParser();
                _cache.Add(site, filterParser);
            }
        }      
        return filterParser;
    }
}
