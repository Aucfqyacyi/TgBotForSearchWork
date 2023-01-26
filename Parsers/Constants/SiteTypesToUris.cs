namespace Parsers.Constants;

public static class SiteTypesToUris
{
    private static readonly Dictionary<SiteType, Uri> _all = new();  
    public static IReadOnlyDictionary<SiteType, Uri> All { get => _all; }

    private static readonly Dictionary<SiteType, string> _theirHosts = new();
    public static IReadOnlyDictionary<SiteType, string> TheirHosts { get => _theirHosts; }

    public const string Https = @"https://";

    static SiteTypesToUris()
    {      
        AddUri(SiteType.WorkUa, new Uri(Https + "www.work.ua/jobs/?advs=1"));
        AddUri(SiteType.Djinni, new Uri(Https + "djinni.co/jobs/"));
        AddUri(SiteType.Dou, new Uri(Https + "jobs.dou.ua/vacancies/"));        
    }

    private static void AddUri(SiteType siteType, Uri uri)
    {
        _all.Add(siteType, uri);
        _theirHosts.Add(siteType, uri.Host);
    }
}