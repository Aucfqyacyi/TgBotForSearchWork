using Parsers.Extensions;
using Parsers.FilterParsers;
using TgBotForSearchWork.Core;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;


if (CommandLineArgs.Token.IsNullOrEmpty())
{
    Console.WriteLine("Argument '--token' missed.");
    return;
}
Log.Info("Application started.");
TelegramBot telegramBot = new(CommandLineArgs.Token, TimeSpan.FromSeconds(CommandLineArgs.Time));
Console.CancelKeyPress += telegramBot.StopEvent;
await telegramBot.StartAsync();
Log.Info("Application stopped.");

