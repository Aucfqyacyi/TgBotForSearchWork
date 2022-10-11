using TgBotForSearchWork.VacancyParsers.Constants;

namespace TgBotForSearchWork.VacancyParsers.VacancyParsers;

public static class VacancyParserFactory
{
    public static IAllVacancyParser CreateAllVacancyParser(Uri uri)
    {
        if (Host.All[Site.Dou] == uri.Host)
            return new DouVacancyParser();
        if (Host.All[Site.Djinni] == uri.Host)
            return new DjinniVacancyParser();
        throw new Exception($"Host({uri.Host}) was not found");
    }
}
