using Parsers.Utilities;

namespace Parsers.IntegrationTests.UtilitiesTests;

public class GlobalHttpClientTests
{
    [Theory]
    [InlineData("https://djinni.co/jobs/")]
    [InlineData("https://jobs.dou.ua/vacancies/")]
    [InlineData("https://www.work.ua/jobs/")]
    public async Task GetAsync_ExistUri_Success(string url)
    {
        // Arrange
        Uri uri = new Uri(url);

        //Act
        var stream = await GlobalHttpClient.GetAsync(uri);

        //Assert
        Assert.True(stream.Length > 0);
    }

    [Theory]
    [InlineData("https://djinni.co/jobs-sometext/")]
    [InlineData("https://djinni.co/jobs/sometext/")]
    [InlineData("https://www.work.ua/sometext-jobs/")]
    [InlineData("https://www.work.ua/jobs/sometext/")]
    [InlineData("https://jobs.dou.ua/vacancies/sometext/")]
    [InlineData("https://jobs.dou.ua/vacancies-sometext/")]
    public async Task GetAsync_NotExistOrInvalidUri_ThrowException(string url)
    {
        // Arrange
        Uri uri = new Uri(url);

        //Act

        //Assert
        await Assert.ThrowsAsync<Exception>(async () => await GlobalHttpClient.GetAsync(uri));
    }
}
