﻿using Parsers.Constants;

namespace Parsers.IntegrationTests.FilterParsersTests;

public class DouFilterParserTests : FilterParsersTests
{
    protected override Dictionary<string, int> CategoryNamesToFilterCount { get; } = new()
    {
        { "Пошук", 1 },
        { "Категорії", 50 },
        { "Досвід", 4 },
        { "Місто", 25 },
    };

    public DouFilterParserTests() : base(SiteType.Dou)
    {
    }
}