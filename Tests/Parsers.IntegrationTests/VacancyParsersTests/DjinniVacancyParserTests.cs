using Parsers.VacancyParsers.Parsers;

namespace Parsers.IntegrationTests.VacancyParsersTests;

public class DjinniVacancyParserTests : VacancyParserTests
{
    public DjinniVacancyParserTests() : base(Constants.SiteType.Djinni, "https://djinni.co/jobs/?exp_level=no_exp&primary_keyword=Rust&salary=8500")
    {
    }

}
