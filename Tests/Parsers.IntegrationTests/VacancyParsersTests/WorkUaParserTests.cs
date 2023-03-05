using Parsers.Constants;

namespace Parsers.IntegrationTests.VacancyParsersTests;

public class WorkUaParserTests : VacancyParserTests
{
    public WorkUaParserTests() : base(SiteType.WorkUa, "https://www.work.ua/jobs-hr-recruitment/?advs=1&employment=75&salaryfrom=9&student=1&disability=1")
    {
    }
}
