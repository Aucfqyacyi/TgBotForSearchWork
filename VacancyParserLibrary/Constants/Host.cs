namespace Parsers.Constants;

internal static class Host
{
    private static readonly Dictionary<SiteType, string> _all = new();
    public const string Https = @"https://";
    public static IReadOnlyDictionary<SiteType, string> All { get => _all; }

    static Host()
    {
        _all.Add(SiteType.Dou, "jobs.dou.ua");
        _all.Add(SiteType.Djinni, "djinni.co");
        _all.Add(SiteType.WorkUa, "www.work.ua");
    }
}