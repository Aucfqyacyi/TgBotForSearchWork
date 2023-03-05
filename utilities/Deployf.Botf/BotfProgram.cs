﻿namespace Deployf.Botf;

public class BotfProgram : BotController
{
    public static void StartBot(
        string[] args,
        bool skipHello = false,
        Action<IServiceCollection, IConfiguration>? onConfigure = null,
        Action<IApplicationBuilder, IConfiguration>? onRun = null,
        BotfOptions? options = null)
    {
        var builder = WebApplication.CreateBuilder(args);

        var botOptions = options;

        if(botOptions == null && builder.Configuration["bot"] != null)
        {
            var section = builder.Configuration.GetSection("bot");
            botOptions = section.Get<BotfOptions>();
        }

        var connectionString = builder.Configuration["botf"];
        if (botOptions == null && connectionString != null)
        {
            botOptions = ConnectionString.Parse(connectionString);
        }
        
        if(botOptions == null)
        {
            throw new BotfException("Configuration is not passed. Check the appsettings*.json.\n" +
                "There must be configuration object like `{ \"bot\": { \"Token\": \"BotToken...\" } }`\n" +
                "Or connection string(in root) like `{ \"botf\": \"bot_token?key=value\" }`");
        }

        builder.Services.AddBotf(botOptions);
        builder.Services.AddHttpClient();

        onConfigure?.Invoke(builder.Services, builder.Configuration);

        var app = builder.Build();
        app.UseBotf();

        onRun?.Invoke(app, builder.Configuration);

        app.Run();
    }

    public static void StartBot<TBotService>(
        string[] args,
        bool skipHello = false,
        Action<IServiceCollection, IConfiguration>? onConfigure = null,
        Action<IApplicationBuilder, IConfiguration>? onRun = null) where TBotService : class, IBotUserService
    {
        StartBot(args, skipHello, (svc, cfg) =>
        {
            onConfigure?.Invoke(svc, cfg);
            svc.AddTransient<IBotUserService, TBotService>();
        }, onRun);
    }
}