using Parsers.Constants;
using Parsers.FilterParsers.Parsers;

namespace Parsers.FilterParsers;

public class FilterParserFactory
{
    private readonly static Dictionary<SiteType, IFilterParser> _cache = new();

    public static IFilterParser CreateFilterParser(SiteType site)
    {
        switch (site)
        {
            case SiteType.Dou:
                return CreateFilterParser<DouFilterParser>(site);
            case SiteType.Djinni:
                return CreateFilterParser<DjinniFilterParser>(site);
            case SiteType.WorkUa:
                return CreateFilterParser<WorkUaFilterParser>(site);
        }
        throw new Exception($"Site({site}) was not found");
    }

    private static IFilterParser CreateFilterParser<TParser>(SiteType site)
                                                where TParser : class, IFilterParser, new()
    {
        IFilterParser? filterParser = _cache.GetValueOrDefault(site);
        if (filterParser is null)
        {
            filterParser = new TParser();
            _cache.Add(site, filterParser);
        }
        return filterParser;
    }
}
