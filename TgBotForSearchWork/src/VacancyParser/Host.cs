namespace TgBotForSearchWork.src.VacancyParser;

internal static class Host
{
    public static Uri Dou { get; }
    public static Uri Djinni { get; }
    public static Uri TempDjinni { get; }
    static Host()
	{
        TempDjinni = new(@"https://djinni.co");
        Dou = new(@"https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=1-3");
        Djinni = new(@"https://djinni.co/jobs/?primary_keyword=.NET&exp_level=1y&exp_level=2y&employment=remote");
    }
}
