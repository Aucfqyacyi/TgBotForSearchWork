using Newtonsoft.Json;
using System.Net;

namespace SimpleCloudflareBypass.Utilities;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionHandlerMiddleware(RequestDelegate next)
    {
        _next = next;
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
            var response = context.Response;
            response.ContentType = "application/json";
            response.StatusCode = (int)HttpStatusCode.UnprocessableEntity;

            var result = JsonConvert.SerializeObject(new { message = "Something went wrong." });
            await response.WriteAsync(result);
        }
    }
}
