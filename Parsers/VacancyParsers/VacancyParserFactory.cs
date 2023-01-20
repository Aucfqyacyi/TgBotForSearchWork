using Parsers.Constants;
using Parsers.VacancyParsers.Parsers;

namespace Parsers.VacancyParsers;

public static class VacancyParserFactory
{
    private readonly static Dictionary<SiteType, IVacancyParser> _cache = new();

    public static IVacancyParser CreateVacancyParser(Uri uri)
    {
        if (SiteTypesToUris.All[SiteType.Dou].Host == uri.Host)
            return CreateVacancyParser<DouVacancyParser>(SiteType.Dou);
        if (SiteTypesToUris.All[SiteType.Djinni].Host == uri.Host)
            return CreateVacancyParser<DjinniVacancyParser>(SiteType.Djinni);
        if (SiteTypesToUris.All[SiteType.WorkUa].Host == uri.Host)
            return CreateVacancyParser<WorkUaVacancyParser>(SiteType.WorkUa);
        throw new Exception($"Host({uri.Host}) was not found");
    }

    private static IVacancyParser CreateVacancyParser<TParser>(SiteType site) 
                                                where TParser : class, IVacancyParser, new()
    {
        IVacancyParser? vacancyParser = _cache.GetValueOrDefault(site);
        if (vacancyParser is null)
        {
            vacancyParser = new TParser();
            _cache.Add(site, vacancyParser);
        }          
        return vacancyParser;
    }
}
