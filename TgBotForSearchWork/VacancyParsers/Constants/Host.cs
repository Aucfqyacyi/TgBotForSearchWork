namespace TgBotForSearchWork.VacancyParsers.Constants;

internal static class Host
{
    public const string Https = @"https://";
    public static readonly Dictionary<Site, string> All = new Dictionary<Site, string>();

    static Host()
    {
        All.Add(Site.Dou, "jobs.dou.ua");
        All.Add(Site.Djinni, "djinni.co");
        All.Add(Site.WorkUa, "www.work.ua");
    }
}