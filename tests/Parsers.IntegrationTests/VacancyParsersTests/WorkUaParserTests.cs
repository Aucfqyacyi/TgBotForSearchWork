using Parsers.Constants;
using Parsers.IntegrationTests.Utilities;

namespace Parsers.IntegrationTests.VacancyParsersTests;

public class WorkUaParserTests : VacancyParserTests
{
    public WorkUaParserTests(HttpClientFixture httpClientFixture)
        : base(SiteType.WorkUa, "https://www.work.ua/jobs-hr-recruitment/?advs=1&employment=75&salaryfrom=9&student=1&disability=1", httpClientFixture)
    {
    }
}
