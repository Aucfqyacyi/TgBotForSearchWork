using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;

namespace SimpleCloudflareBypass.Controllers;

public static class Controller
{
    public static object _locker = new();

    public static string Send([FromBody] SendRequest request, [FromServices] ChromeDriverFactory chromeDriverFactory, CancellationToken cancellationToken)
    {
        return GetPageSource(request, chromeDriverFactory, cancellationToken);
    }

    public static IResult SendMany([FromBody] SendManyRequest manyRequest, [FromServices] ChromeDriverFactory chromeDriverFactory, CancellationToken cancellationToken)
    {
        List<string> pageSources = new();
        foreach (var request in manyRequest.Requests)
        {
            pageSources.Add(GetPageSource(request, chromeDriverFactory, cancellationToken));
        }
        return Results.Ok(pageSources);
    }

    private static string GetPageSource(SendRequest request, ChromeDriverFactory chromeDriverFactory, CancellationToken cancellationToken)
    {
        lock (_locker)
        {
            while (true)
            {
                IWebDriver webDriver = chromeDriverFactory.CreateIfCallReboot();
                webDriver.Url = request.Url;
                if (request.IdOnLoadedPage is null)
                    return webDriver.PageSource;
                try
                {
                    Console.WriteLine($"{DateTime.Now}: Processing the url({request.Url}).");
                    WaitUntilResolvingChallenge(webDriver, request.IdOnLoadedPage, request.Timeout, cancellationToken);
                    return webDriver.PageSource;
                }
                catch (WebDriverException ex)
                {
                    Console.WriteLine(ex.Message);
                    chromeDriverFactory.Reboot();
                }
            }
        }
    }

    private static void WaitUntilResolvingChallenge(IWebDriver webDriver, string idOnLoadedPage, int timeout, CancellationToken cancellationToken)
    {
        WebDriverWait wait = new(webDriver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        wait.Until(driver => driver.FindElement(By.Id(idOnLoadedPage)), cancellationToken);
    }
}
