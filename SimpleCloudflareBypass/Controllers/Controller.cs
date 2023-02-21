using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using SimpleCloudflareBypass.Models;

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
            if (request.IdOnLoadedPage is not null)
            {
                try
                {
                    WebDriverWait wait = new(chromeDriver, TimeSpan.FromSeconds(request.Timeout));
                    wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                    wait.Until(driver => driver.FindElement(By.Id(request.IdOnLoadedPage)), cancellationToken);
                }
                catch
                { }
            }
            return chromeDriver.PageSource;
        }                
    }
}
