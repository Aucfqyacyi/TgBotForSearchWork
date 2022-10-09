namespace TgBotForSearchWork.src.VacancyParsers;

public static class VacancyParserFactory
{
    public static VacancyParser Create(string host)
    {
        switch (host)
        {
            case Host.Dou:
                return new DouVacancyParser();
            case Host.Djinni:
                return new DjinniVacancyParser();
            default:
                throw new Exception($"Host({host}) was not found");
        }
    }

    public static Dictionary<string, VacancyParser> Create(IEnumerable<string> hosts)
    {
        Dictionary<string, VacancyParser> parsers = new();
        foreach (var host in hosts)
        {
            parsers.Add(host, Create(host));
        }
        return parsers;
    }

    public static VacancyParser Create(Uri uri)
    {
        return Create(uri.Host);
    }

    public static Dictionary<string, VacancyParser> Create(IEnumerable<Uri> uris)
    {
        return Create(uris.Select(e => e.Host));
    }

}
