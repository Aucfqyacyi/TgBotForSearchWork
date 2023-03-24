using Parsers.Constants;
using Parsers.VacancyParsers;
using Parsers.VacancyParsers.Parsers;

namespace Parsers.ParserFactories;

public class VacancyParserFactory : ParserFactory<IVacancyParser>
{
    public VacancyParserFactory(HttpClient httpClient) : base(httpClient)
    {
    }

    public override IVacancyParser Create(SiteType siteType)
    {
        switch (siteType)
        {
            case SiteType.Dou:
                return new DouVacancyParser();
            case SiteType.Djinni:
                return new DjinniVacancyParser(_httpClient);
            case SiteType.WorkUa:
                return new WorkUaVacancyParser(_httpClient);
        }
        throw new Exception($"Parser with sitetype({siteType}) was not found.");
    }
}
