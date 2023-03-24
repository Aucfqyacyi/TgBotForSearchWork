using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Http;
using Parsers.ParserFactories;
using Parsers.Utilities;

namespace TgBotForSearchWorkApi.Extensions;

public static class IServiceCollectionExtension
{
    public static IServiceCollection AddNamedHttpClient(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpClient(nameof(HttpClient))
                .ConfigurePrimaryHttpMessageHandler(provider =>
                {
                    string? SimpleCloudflareBypassUrl = configuration.GetValue<string>(nameof(SimpleCloudflareBypassUrl));
                    if (SimpleCloudflareBypassUrl is null)
                        throw new Exception($"{nameof(SimpleCloudflareBypassUrl)} doesn't exist at the appsettings.json.");
                    return new SimplyCloudflareBypassHandler(SimpleCloudflareBypassUrl);
                })
                .ConfigureHttpClient(httpClient =>
                {
                    httpClient.Timeout = TimeSpan.FromMinutes(2);
                    httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X x.y; rv:42.0) Gecko/20100101 Firefox/42.0");
                });
        services.RemoveAll<IHttpMessageHandlerBuilderFilter>();
        return services;
    }

    public static IServiceCollection AddParserFactories(this IServiceCollection services)
    {
        return services.AddSingleton<VacancyParserFactory>(provider => new(GetHttpClientFromServices(provider)))
                       .AddSingleton<FilterParserFactory>(provider => new(GetHttpClientFromServices(provider)));
    }

    private static HttpClient GetHttpClientFromServices(IServiceProvider serviceProvider)
    {
        var httpClienFactory = serviceProvider.GetRequiredService<IHttpClientFactory>();
        return httpClienFactory.CreateClient(nameof(HttpClient));
    }
}
