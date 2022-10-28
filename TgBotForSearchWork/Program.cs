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
    

FileManager fileManager = new("Users.txt");
UserManager userManager = new(fileManager);
userManager.AddDefaultUser();
TelegramBot telegramBot = new(CommandLineArgs.Token, TimeSpan.FromSeconds(CommandLineArgs.Time), userManager);
telegramBot.Start();
while (Console.ReadLine() != "111") ;
telegramBot.Stop();

