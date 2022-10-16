using TgBotForSearchWork.VacancyParsers.Constants;
using TgBotForSearchWork.VacancyParsers.VacancyParsers.Parsers;

namespace TgBotForSearchWork.VacancyParsers.VacancyParsers;

public static class VacancyParserFactory
{
    public static IAllVacancyParser CreateAllVacancyParser(Uri uri)
    {
        if (Host.All[Site.Dou] == uri.Host)
            return new DouVacancyParser();
        if (Host.All[Site.Djinni] == uri.Host)
            return new DjinniVacancyParser();
        if (Host.All[Site.WorkUa] == uri.Host)
            return new WorkUaVacancyParser();
        throw new Exception($"Host({uri.Host}) was not found");
    }
}
