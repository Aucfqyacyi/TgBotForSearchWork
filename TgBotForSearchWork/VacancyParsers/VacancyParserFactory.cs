using TgBotForSearchWork.Constants;
using TgBotForSearchWork.VacancyParsers.Parsers;

namespace TgBotForSearchWork.VacancyParsers;

public static class VacancyParserFactory
{
    private readonly static Dictionary<Site, IVacancyParser> _cache = new();

    public static IVacancyParser CreateVacancyParser(Uri uri)
    {
        if (Host.All[Site.Dou] == uri.Host)
            return CreateVacancyParser<DouVacancyParser>(Site.Dou);
        if (Host.All[Site.Djinni] == uri.Host)
            return CreateVacancyParser<DjinniVacancyParser>(Site.Djinni);
        if (Host.All[Site.WorkUa] == uri.Host)
            return CreateVacancyParser<WorkUaVacancyParser>(Site.WorkUa);
        throw new Exception($"Host({uri.Host}) was not found");
    }

    private static IVacancyParser CreateVacancyParser<TParser>(Site site) 
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
