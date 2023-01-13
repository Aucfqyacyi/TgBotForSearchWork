namespace TgBotForSearchWork.Constants;

internal static class Host
{
    private static readonly Dictionary<Site, string> _all = new();
    public const string Https = @"https://";
    public static IReadOnlyDictionary<Site, string> All { get => _all; }

    static Host()
    {
        _all.Add(Site.Dou, "jobs.dou.ua");
        _all.Add(Site.Djinni, "djinni.co");
        _all.Add(Site.WorkUa, "www.work.ua");
    }
}