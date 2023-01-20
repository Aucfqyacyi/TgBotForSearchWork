namespace Parsers.Constants;

public static class SiteTypesToUris
{
    private static readonly Dictionary<SiteType, Uri> _all = new();
    public const string Https = @"https://";
    public static IReadOnlyDictionary<SiteType, Uri> All { get => _all; }

    static SiteTypesToUris()
    {
        _all.Add(SiteType.WorkUa, new Uri(Https + "www.work.ua/jobs/?advs=1"));
        _all.Add(SiteType.Djinni, new Uri(Https + "djinni.co/jobs/"));
        _all.Add(SiteType.Dou, new Uri(Https + "jobs.dou.ua/vacancies/"));
    }
}