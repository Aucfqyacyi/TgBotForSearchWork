using TgBotForSearchWork.Core;
using TgBotForSearchWork.Extensions;
using TgBotForSearchWork.Utilities;
using TgBotForSearchWork.Services;


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

