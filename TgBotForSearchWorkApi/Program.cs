using Deployf.Botf;
using MongoDB.Driver;
using TgBotForSearchWorkApi.Extensions;
using TgBotForSearchWorkApi.Services;

FilterService filterService = new();
await filterService.CollectFiltersAsync();

BotfProgram.StartBot(args, onConfigure: (services, congif) =>
{
    string? MongoDbConnetionString = congif.GetSection(nameof(MongoDbConnetionString)).Value;
    string? MongoDatabaseName = congif.GetSection(nameof(MongoDatabaseName)).Value;

    ArgumentNullException.ThrowIfNull(MongoDbConnetionString, nameof(MongoDbConnetionString));
    ArgumentNullException.ThrowIfNull(MongoDatabaseName, nameof(MongoDatabaseName));

    MongoClient mongoClient = new(MongoDbConnetionString);

    services.AddSingleton(mongoClient)
            .AddSingleton(mongoClient.GetDatabase(MongoDatabaseName))
            .AddSingleton(filterService)
            .AddServices()
            .AddHostedService<VacancyBackgroundService>();

}, onRun: (app, congif) =>
{
    
});