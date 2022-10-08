namespace TgBotForSearchWork.src.VacancyParser;

public static class VacancyParserFactory
{
    public static VacancyParser Create(Uri uri)
    {
        switch (uri.Host)
        {
            case Host.DouH:
                return new DouVacancyParser();
            case Host.DjinniH:
                return new DjinniVacancyParser();
            default:
                throw new Exception($"Host({uri.Host}) was not found");
        }
    }
}
