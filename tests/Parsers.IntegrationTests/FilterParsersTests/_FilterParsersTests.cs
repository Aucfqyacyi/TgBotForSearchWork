using Parsers.Constants;
using Parsers.FilterParsers;
using Parsers.IntegrationTests.Utilities;
using Parsers.Models;
using Parsers.ParserFactories;

namespace Parsers.IntegrationTests.FilterParsersTests;


public abstract class FilterParsersTests : IClassFixture<HttpClientFixture>
{
    protected readonly Uri _incorrectUri;
    protected readonly Uri _correctUri;
    protected readonly SiteType _siteType;
    protected readonly IFilterParser _filterParser;

    protected abstract Dictionary<string, int> CategoryNamesToFilterCount { get; }

    protected FilterParsersTests(SiteType siteType, HttpClientFixture httpClientFixture)
    {
        _siteType = siteType;
        Uri uri = SiteTypesToUris.All[_siteType];
        _incorrectUri = new(uri.OriginalString.Replace(uri.PathAndQuery, "/something/"));
        _correctUri = SiteTypesToUris.All[_siteType];
        FilterParserFactory filterParserFactory = new(httpClientFixture.Client);
        _filterParser = filterParserFactory.Create(_siteType);
    }

    [Fact]
    public async Task ParseAsync_CorrectUri_CorrectFilters()
    {
        // Arrange
        //Act
        List<Filter> filters = await _filterParser.ParseAsync(_correctUri);
        var categoryNameToFilterCount = filters.Aggregate(new Dictionary<string, int>(), (categoryNameToFilterCount, filter) =>
        {
            if (categoryNameToFilterCount.ContainsKey(filter.Category.Name) is false)
                categoryNameToFilterCount.Add(filter.Category.Name, 0);
            categoryNameToFilterCount[filter.Category.Name] += 1;
            return categoryNameToFilterCount;
        });

        //Assert
        Assert.Equivalent(categoryNameToFilterCount, CategoryNamesToFilterCount);
    }

    [Fact]
    public async Task ParseAsync_IncorrectUri_ThrowException()
    {
        // Arrange
        //Act

        //Assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await _filterParser.ParseAsync(_incorrectUri));
    }
}
