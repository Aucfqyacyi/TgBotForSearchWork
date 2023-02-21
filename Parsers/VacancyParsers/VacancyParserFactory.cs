using AngleSharp.Io;
using Parsers.Constants;
using Parsers.VacancyParsers.Parsers;

namespace Parsers.VacancyParsers;

public static class VacancyParserFactory
{
    private readonly static Dictionary<SiteType, IVacancyParser> _cache = new();
    private readonly static object _lock = new object();

    public static IVacancyParser Create(Uri uri)
    {
        return Create(SiteTypesToUris.HostsToSiteTypes[uri.Host]);
    }

    public static IVacancyParser Create(SiteType siteType)
    {
        switch (siteType)
        {
            case SiteType.Dou:
                return Create<DouVacancyParser>(siteType);
            case SiteType.Djinni:
                return Create<DjinniVacancyParser>(siteType);
            case SiteType.WorkUa:
                return Create<WorkUaVacancyParser>(siteType);
        }
        throw new Exception($"Parser with sitetype({siteType}) was not found.");
    }

    private static IVacancyParser Create<TParser>(SiteType site) 
                                                where TParser : class, IVacancyParser, new()
    {
        IVacancyParser? vacancyParser = null;
        lock (_lock)
        {
            vacancyParser = _cache.GetValueOrDefault(site);
            if (vacancyParser is null)
            {
                vacancyParser = new TParser();
                _cache.Add(site, vacancyParser);
            }
        }            
        return vacancyParser;
    }
}
