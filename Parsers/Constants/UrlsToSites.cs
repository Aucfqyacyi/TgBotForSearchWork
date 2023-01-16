namespace Parsers.Constants;

internal static class UrlsToSites
{
    private static readonly Dictionary<SiteType, string> _hosts = new();
    public const string Https = @"https://";
    public static IReadOnlyDictionary<SiteType, string> Hosts { get => _hosts; }

    static UrlsToSites()
    {
        _hosts.Add(SiteType.Dou, "jobs.dou.ua");
        _hosts.Add(SiteType.Djinni, "djinni.co");
        _hosts.Add(SiteType.WorkUa, "www.work.ua");
    }

    public static string GetFullUrlToSite(SiteType siteType, string? aditionalPath = null)
    {
        return Https + Hosts[siteType] + aditionalPath;
    }
}