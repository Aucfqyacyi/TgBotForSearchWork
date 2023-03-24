using Parsers.Constants;

namespace Parsers.ParserFactories;

public abstract class ParserFactory<TParser> where TParser : class
{
    protected readonly SortedDictionary<SiteType, TParser> _cache = new();
    protected readonly SemaphoreSlim _semaphoreSlim = new(1);
    protected readonly HttpClient _httpClient;

    protected ParserFactory(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public abstract TParser Create(SiteType siteType);

    public TParser Create(Uri uri)
    {
        return Create(SiteTypesToUris.HostsToSiteTypes[uri.Host]);
    }

    public TParser GetOrCreate(SiteType site)
    {
        _semaphoreSlim.Wait();
        TParser? vacancyParser = _cache.GetValueOrDefault(site);
        if (vacancyParser is null)
        {
            vacancyParser = Create(site);
            _cache.Add(site, vacancyParser);
        }
        _semaphoreSlim.Release();
        return vacancyParser;
    }

    public TParser GetOrCreate(Uri uri)
    {
        return GetOrCreate(SiteTypesToUris.HostsToSiteTypes[uri.Host]);
    }
}
