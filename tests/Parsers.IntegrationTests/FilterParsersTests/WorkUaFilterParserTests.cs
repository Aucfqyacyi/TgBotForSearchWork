using Parsers.Constants;

namespace Parsers.IntegrationTests.FilterParsersTests;

public class WorkUaFilterParserTests : FilterParsersTests
{
    protected override Dictionary<string, int> CategoryNamesToFilterCount { get; } = new()
    {
        {"Пошук", 1 },
        {"Міста", 27 },
        {"Категорія", 28 },
        {"Вид зайнятості", 2 },
        {"Зарплата від", 9 },
        {"Зарплата до", 9 },
        {"Зарплата не вказана", 1 },
        {"Підходить", 6 },
        {"Пошуковий запит", 3 },
    };

    public WorkUaFilterParserTests() : base(SiteType.WorkUa)
    {
    }

}
