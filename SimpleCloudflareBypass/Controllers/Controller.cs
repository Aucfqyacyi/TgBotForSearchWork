using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;

namespace SimpleCloudflareBypass.Controllers;

public static class Controller
{
    public static object _locker = new();

    public static string Send([FromBody] SendRequest request, [FromServices] IWebDriver webDriver, CancellationToken cancellationToken)
    {       
        return GetPageSource(request, webDriver, cancellationToken);
    }

    public static IResult SendMany([FromBody] SendManyRequest manyRequest, [FromServices] IWebDriver webDriver, CancellationToken cancellationToken)
    {
        List<string> pageSources = new();
        foreach (var request in manyRequest.Requests)
        {
            pageSources.Add(GetPageSource(request, webDriver, cancellationToken));
        }
        return Results.Ok(pageSources);
    }

    private static string GetPageSource(SendRequest request, IWebDriver webDriver, CancellationToken cancellationToken)
    {
        lock (_locker)
        {
            webDriver.Url = request.Url;
            while (request.IdOnLoadedPage is not null)
            {
                try
                {
                    Console.WriteLine($"{DateTime.Now}: Processing the url({request.Url}).");
                    WaitUntilResolvingChallenge(webDriver, request.IdOnLoadedPage, request.Timeout, cancellationToken);
                    break;
                }
                catch (WebDriverException)
                {
                    ChromeDriver.Reboot(webDriver);
                    webDriver.Url = request.Url;
                }
            }
            return webDriver.PageSource;
        }                
    }

    private static void WaitUntilResolvingChallenge(IWebDriver webDriver, string idOnLoadedPage, int timeout, CancellationToken cancellationToken)
    {
        WebDriverWait wait = new(webDriver, TimeSpan.FromSeconds(timeout));
        wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
        wait.Until(driver => driver.FindElement(By.Id(idOnLoadedPage)), cancellationToken);
    }
}
