using SimpleCloudflareBypass.Controllers;
using SimpleCloudflareBypass.Models;
using SimpleCloudflareBypass.Utilities;



var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton(ChromeDriverCreator.Create());

var app = builder.Build();

app.MapPost("/send", Controller.Send)
   .Accepts<Request>("application/json")
   .AddEndpointFilter<EndpointFilter>()
   .AllowAnonymous();

app.MapPost("/sendMany", Controller.SendMany)
   .Accepts<ManyRequest>("application/json")
   .AddEndpointFilter<EndpointFilter>()
   .AllowAnonymous();

app.UseMiddleware<ExceptionHandlerMiddleware>();

app.Run();
