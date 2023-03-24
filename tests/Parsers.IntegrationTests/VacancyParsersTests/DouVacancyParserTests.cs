using Parsers.IntegrationTests.Utilities;

namespace Parsers.IntegrationTests.VacancyParsersTests;

public class DouVacancyParserTests : VacancyParserTests
{
    public DouVacancyParserTests(HttpClientFixture httpClientFixture)
        : base(Constants.SiteType.Dou, "https://jobs.dou.ua/vacancies/feeds/?category=Scala&city=%D0%A5%D0%B5%D1%80%D1%81%D0%BE%D0%BD", httpClientFixture)
    {
    }

}
