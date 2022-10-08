namespace TgBotForSearchWork.src.VacancyParser;

internal static class Host
{
    public const string Https = @"https://";
    public const string DouH = "jobs.dou.ua";
    public const string DjinniH = "djinni.co";
    public static Uri Dou { get; }
    public static Uri Djinni { get; }


    static Host()
    {
        Dou = new(@"https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=1-3");
        Djinni = new(@"https://djinni.co/jobs/?primary_keyword=.NET&exp_level=1y&exp_level=2y&employment=remote");
    }
}