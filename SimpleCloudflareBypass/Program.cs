using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ChromeDriverFactory>();

var app = builder.Build();

app.MapPost("/send", SimpleCloudflareBypass.Controllers.Controller.Send)
   .Accepts<SendRequest>("application/json")
   .AddEndpointFilter<EndpointFilter>()
   .AllowAnonymous();

app.MapPost("/sendMany", SimpleCloudflareBypass.Controllers.Controller.SendMany)
   .Accepts<SendManyRequest>("application/json")
   .AddEndpointFilter<EndpointFilter>()
   .AllowAnonymous();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();
