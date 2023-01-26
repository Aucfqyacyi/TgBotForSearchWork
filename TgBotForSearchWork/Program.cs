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

UserService userService = new();
userService.AddDefaultUser();

CommandHandler commandHandler = new(userService);
VacancySender vacancySender = new(userService);

TelegramBot telegramBot = new(CommandLineArgs.Token, TimeSpan.FromSeconds(CommandLineArgs.Time), vacancySender, commandHandler);
Console.CancelKeyPress += telegramBot.StopEvent;
await telegramBot.StartAsync();
telegramBot.Stop();

Log.Info("Application stopped.");

