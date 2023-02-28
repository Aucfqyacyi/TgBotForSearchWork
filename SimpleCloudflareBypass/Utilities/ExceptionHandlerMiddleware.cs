using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Net;

namespace SimpleCloudflareBypass.Utilities;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebDriver _webDriver;

    public ExceptionHandlerMiddleware(RequestDelegate next, IWebDriver webDriver)
    {
        _next = next;
        _webDriver = webDriver;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"In endpoint was throwed exception, error = {ex.Message}.");
            await WriteToResponseAsync(context, HttpStatusCode.UnprocessableEntity, "Something went wrong.");
        }
    }

    private async Task WriteToResponseAsync(HttpContext context, HttpStatusCode httpStatusCode, string message)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)httpStatusCode;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new { message }));
    }
}
