namespace Parsers.Constants;

public static class SiteTypesToUris
{
    private static readonly Dictionary<SiteType, Uri> _all = new();  
    public static IReadOnlyDictionary<SiteType, Uri> All { get => _all; }

    private static readonly Dictionary<string, SiteType> _hostsToSiteTypes = new();
    public static IReadOnlyDictionary<string, SiteType> HostsToSiteTypes { get => _hostsToSiteTypes; }

    static SiteTypesToUris()
    {
        AddUri(SiteType.WorkUa, new Uri(Uri.UriSchemeHttps + Uri.SchemeDelimiter + "www.work.ua/jobs/?advs=1"));
        AddUri(SiteType.Djinni, new Uri(Uri.UriSchemeHttps + Uri.SchemeDelimiter + "djinni.co/jobs/"));
        AddUri(SiteType.Dou, new Uri(Uri.UriSchemeHttps + Uri.SchemeDelimiter + "jobs.dou.ua/vacancies/"));        
    }

    private static void AddUri(SiteType siteType, Uri uri)
    {
        _all.Add(siteType, uri);
        _hostsToSiteTypes.Add(uri.Host, siteType);
    }
}