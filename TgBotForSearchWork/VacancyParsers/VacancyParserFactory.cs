using TgBotForSearchWork.Core.Constants;
using TgBotForSearchWork.VacancyParsers.AllVacancyParsers;
using TgBotForSearchWork.VacancyParsers.Constants;
using TgBotForSearchWork.VacancyParsers.DetailVacancyVarsers;

namespace TgBotForSearchWork.VacancyParsers;

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

    public static IDetailVacancyParser CreateDetailVacancyParser(Uri uri)
    {
        return new DetailVacancyParser();
        throw new Exception($"Host({uri.Host}) was not found");
    }

}
