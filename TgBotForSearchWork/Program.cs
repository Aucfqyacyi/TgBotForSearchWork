using Parsers.Extensions;
using TgBotForSearchWork.Core;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;



if (CommandLineArgs.Token.IsNullOrEmpty())
{
    Console.WriteLine("Argument '--token' missed.");
    return;
}

Log.Info("Application started.");

UserService UserService = new();
UserService.AddDefaultUser();

FilterService FilterService = new();
await FilterService.CollectFiltersAsync();

TelegramBot telegramBot = new(CommandLineArgs.Token, TimeSpan.FromSeconds(CommandLineArgs.Time), UserService);
Console.CancelKeyPress += telegramBot.StopEvent;
await telegramBot.StartAsync();
telegramBot.Stop();

Log.Info("Application stopped.");
