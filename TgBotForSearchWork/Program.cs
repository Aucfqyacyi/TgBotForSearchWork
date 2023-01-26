using Parsers.Constants;
using Parsers.Extensions;
using System.Diagnostics;
using System.Text;
using TgBotForSearchWork.Core;
using TgBotForSearchWork.Core.CommandHandlers;
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

FilterService filterService = new();
await filterService.CollectFiltersAsync();
BuildUrlCommandHandler buildUrlCommandHandler = new(filterService);
CommandHandler commandHandler = new(userService, buildUrlCommandHandler);
VacancySender vacancySender = new(userService);

TelegramBot telegramBot = new(CommandLineArgs.Token, TimeSpan.FromSeconds(CommandLineArgs.Time), vacancySender, commandHandler);
Console.CancelKeyPress += telegramBot.StopEvent;
await telegramBot.StartAsync();
telegramBot.Stop();

Log.Info("Application stopped.");

