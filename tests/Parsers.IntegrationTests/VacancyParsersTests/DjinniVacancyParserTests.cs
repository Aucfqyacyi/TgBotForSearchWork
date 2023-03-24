using Parsers.IntegrationTests.Utilities;

namespace Parsers.IntegrationTests.VacancyParsersTests;

public class DjinniVacancyParserTests : VacancyParserTests
{
    public DjinniVacancyParserTests(HttpClientFixture httpClientFixture)
                            : base(Constants.SiteType.Djinni, "https://djinni.co/jobs/?exp_level=no_exp&primary_keyword=Rust&salary=8500", httpClientFixture)
    {
    }

}
