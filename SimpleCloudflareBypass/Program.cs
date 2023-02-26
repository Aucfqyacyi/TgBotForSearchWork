using SimpleCloudflareBypass.Controllers;
using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(ChromeDriver.Create());

var app = builder.Build();

app.MapPost("/send", Controller.Send)
   .Accepts<SendRequest>("application/json")
   .AddEndpointFilter<EndpointFilter>()
   .AllowAnonymous();

app.MapPost("/sendMany", Controller.SendMany)
   .Accepts<SendManyRequest>("application/json")
   .AddEndpointFilter<EndpointFilter>()
   .AllowAnonymous();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();
