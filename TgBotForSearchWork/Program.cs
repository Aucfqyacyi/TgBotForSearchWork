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
UserService userManager = new();
userManager.AddDefaultUser();
TelegramBot telegramBot = new(CommandLineArgs.Token, TimeSpan.FromSeconds(CommandLineArgs.Time), userManager);
await telegramBot.StartAsync();
Log.Info("Application stopped.");

