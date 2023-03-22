using Parsers.Constants;

namespace Parsers.IntegrationTests.FilterParsersTests;

public class DjinniFilterParserTests : FilterParsersTests
{
    protected override Dictionary<string, int> CategoryNamesToFilterCount { get; } = new()
    {
        {"Пошук", 1},
        {"Job title", 41}, {"Спеціалізація" , 41},
        {"Country", 5}, {"Країна" , 5},
        {"Work experience", 5}, {"Досвід роботи" , 5},
        {"Employment", 3}, {"Зайнятість" , 3},
        {"Company type", 4}, {"Тип компанії" , 4},
        {"Salary from", 8}, {"Зарплата від" , 8},
        {"English", 6}, {"Англіська" , 6},
        {"Editorial jobs", 5}, {"Добірки вакансій" , 5},
    };

    public DjinniFilterParserTests() : base(SiteType.Djinni)
    {
    }

}
