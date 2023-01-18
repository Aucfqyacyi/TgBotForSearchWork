namespace Parsers.Constants;

public static class UrisToSites
{
    private static readonly Dictionary<SiteType, Uri> _uris = new();
    public const string Https = @"https://";
    public static IReadOnlyDictionary<SiteType, Uri> Uris { get => _uris; }

    static UrisToSites()
    {
        _uris.Add(SiteType.Dou, new Uri(Https + "www.work.ua/vacancies/"));
        _uris.Add(SiteType.Djinni, new Uri(Https + "djinni.co/jobs/"));
        _uris.Add(SiteType.WorkUa, new Uri(Https + "jobs.dou.ua/jobs/?advs=1"));
    }
}