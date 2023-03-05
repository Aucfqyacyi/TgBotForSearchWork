using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;
using Parsers.IntegrationTests.Utilities;
using Parsers.VacancyParsers;

namespace Parsers.IntegrationTests.VacancyParsersTests;

public abstract class VacancyParserTests
{
    protected readonly Uri _incorrectUri;
    protected readonly Uri _correctUri;
    protected readonly Uri _uriWithoutVacancies;
    protected readonly SiteType _siteType;
    protected readonly IVacancyParser _vacancyParser;

    protected VacancyParserTests(SiteType siteType, string urlWithoutVacancies)
    {
        _siteType = siteType;
        Uri uri = SiteTypesToUris.All[_siteType];
        _incorrectUri = new(uri.OriginalString.Replace(uri.PathAndQuery, "/something/"));
        _correctUri = SiteTypesToUris.All[_siteType];
        _vacancyParser = VacancyParserFactory.Create(_siteType);
        _uriWithoutVacancies = new(urlWithoutVacancies);
    }

    [Fact]
    public async Task ParseAsync_CorrectUri_CorrectParsedVacancies()
    {
        //Arrange
        //Act
        var vacanciesWithDescription = _vacancyParser.ParseAsync(_correctUri, 1000).AsTask();
        var vacanciesWithoutDescription = _vacancyParser.ParseAsync(_correctUri, 0).AsTask();
        await Task.WhenAll(vacanciesWithDescription, vacanciesWithoutDescription);

        //Assert
        Assert.NotEmpty(vacanciesWithDescription.Result);
        Assert.All(vacanciesWithDescription.Result, vac =>
        {
            Assert.True(vac.Id != 0);
            Assert.True(vac.Title.IsNotNullOrEmpty());
            Assert.True(vac.Description.IsNotNullOrEmpty());
            Assert.True(vac.Url.IsNotNullOrEmpty());
        });

        Assert.NotEmpty(vacanciesWithoutDescription.Result);
        Assert.All(vacanciesWithoutDescription.Result, vac =>
        {
            Assert.True(vac.Id != 0);
            Assert.True(vac.Title.IsNotNullOrEmpty());
            Assert.True(vac.Description.IsNullOrEmpty());
            Assert.True(vac.Url.IsNotNullOrEmpty());
        });
    }

    [Fact]
    public async Task ParseAsync_UriWithoutVacancis_EmptyList()
    {
        //Arrange
        //Act
        var vacancies = await _vacancyParser.ParseAsync(_uriWithoutVacancies, 1000);
        //Assert
        Assert.Empty(vacancies);
    }

    [Fact]
    public async Task ParseAsync_IncorrectUri_ThrowException()
    {
        //Arrange
        //Act
        //Assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await _vacancyParser.ParseAsync(_incorrectUri, 1000));
    }

    [Fact]
    public async Task ParseAsync_WithVacancyIdsToIgnore_IgnoringSomeVacancies()
    {
        //Arrange
        int vacanciesToIngoreCount = 5;
        IEnumerable<Vacancy> vacanciesToIngore = await _vacancyParser.ParseAsync(_correctUri, 1000);
        IEnumerable<ulong> initialVacancyIdsToIgnore = vacanciesToIngore.Take(vacanciesToIngoreCount)
                                                                        .Select(vac => vac.Id);
        IEnumerable<ulong> middleVacancyIdsToIgnore = vacanciesToIngore.Skip(vacanciesToIngoreCount)
                                                                       .Take(vacanciesToIngoreCount)
                                                                       .Select(vac => vac.Id);

        //Act
        var initialVacancies = _vacancyParser.ParseAsync(_correctUri, 1000, initialVacancyIdsToIgnore.ToList()).AsTask();
        var middleVacancies = _vacancyParser.ParseAsync(_correctUri, 1000, middleVacancyIdsToIgnore.ToList()).AsTask();
        await Task.WhenAll(initialVacancies, middleVacancies);

        //Assert
        Assert.Empty(initialVacancies.Result);
        Assert.Equal(middleVacancies.Result.Count(), vacanciesToIngoreCount);
        foreach (var vacancyToIngore in vacanciesToIngore.Skip(vacanciesToIngoreCount))
        {
            Assert.DoesNotContain(vacancyToIngore, middleVacancies.Result, new VacancyComparer());
        }
    }

    [Fact]
    public async Task IsCorrectUriAsync_CorrectUri_True()
    {
        //Arrange
        //Act
        bool result = await _vacancyParser.IsCorrectUriAsync(_correctUri);

        //Assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsCorrectUriAsync_IncorrectUri_False()
    {
        // Arrange
        //Act
        bool result = await _vacancyParser.IsCorrectUriAsync(_incorrectUri);

        //Assert
        Assert.False(result);
    }
}
