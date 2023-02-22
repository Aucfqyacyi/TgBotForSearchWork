using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;

namespace SimpleCloudflareBypass.Controllers;

public static class Controller
{
    public static object _locker = new();

    public static string Send([FromBody] SendRequest request, [FromServices] IWebDriver chromeDriver, CancellationToken cancellationToken)
    {       
        return GetPageSource(request, chromeDriver, cancellationToken);
    }

    public static IResult SendMany([FromBody] SendManyRequest manyRequest, [FromServices] IWebDriver chromeDriver, CancellationToken cancellationToken)
    {
        List<string> pageSources = new();
        foreach (var request in manyRequest.Requests)
        {
            pageSources.Add(GetPageSource(request, chromeDriver, cancellationToken));
        }
        return Results.Ok(pageSources);
    }

    private static string GetPageSource(SendRequest request, IWebDriver chromeDriver, CancellationToken cancellationToken)
    {
        lock (_locker)
        {
            chromeDriver.Url = request.Url;
            while (request.IdOnLoadedPage is not null)
            {
                try
                {
                    WaitUntilResolvingChallenge(chromeDriver, request.IdOnLoadedPage, request.Timeout, cancellationToken);
                    break;
                }
                catch (WebDriverTimeoutException ex)
                {
                    chromeDriver.Dispose();
                    chromeDriver = ChromeDriverCreator.Create();
                    chromeDriver.Url = request.Url;
                }
            }
            return chromeDriver.PageSource;
        }                
    }

    private static void WaitUntilResolvingChallenge(IWebDriver chromeDriver, string idOnLoadedPage, int timeout, CancellationToken cancellationToken)
    {
        WebDriverWait wait = new(chromeDriver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        wait.Until(driver => driver.FindElement(By.Id(idOnLoadedPage)), cancellationToken);
    }
}
