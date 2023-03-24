using Parsers.Constants;
using Parsers.FilterParsers;
using Parsers.FilterParsers.Parsers;

namespace Parsers.ParserFactories;

public class FilterParserFactory : ParserFactory<IFilterParser>
{
    public FilterParserFactory(HttpClient httpClient) : base(httpClient)
    {
    }

    public override IFilterParser Create(SiteType sitetype)
    {
        switch (sitetype)
        {
            case SiteType.Dou:
                return new DouFilterParser(_httpClient);
            case SiteType.Djinni:
                return new DjinniFilterParser(_httpClient);
            case SiteType.WorkUa:
                return new WorkUaFilterParser(_httpClient);
        }
        throw new Exception($"Parser with sitetype({sitetype}) was not found.");
    }

}
