using TgBotForSearchWork.Core;
using TgBotForSearchWork.Managers.UserManagers;
using TgBotForSearchWork.Managers.FileManagers;
using TgBotForSearchWork.Others;
using TgBotForSearchWork.Extensions;



if (CommandLineArgs.Token.IsNullOrEmpty())
{
    Console.WriteLine("Argument '--token' missed.");
    return;
}
Log.Info("Application started.");
FileManager fileManager = new("Users.txt");
UserManager userManager = new(fileManager);
userManager.AddDefaultUser();
TelegramBot telegramBot = new(CommandLineArgs.Token, TimeSpan.FromSeconds(CommandLineArgs.Time), userManager);
await telegramBot.StartAsync();
Log.Info("Application stopped.");

