using TgBotForSearchWork.Core;
using TgBotForSearchWork.Managers.UserManagers;
using TgBotForSearchWork.Managers.FileManagers;

FileManager fileManager = new("Users.txt");
UserManager userManager = new(fileManager);
TelegramBot telegramBot = new("2128837514:AAEJVAT5VUFQcXGo03tsCj0KobKns8HQRgU", TimeSpan.FromSeconds(15), userManager);
telegramBot.Start();
while (Console.ReadLine() != "111") ;
telegramBot.Stop();

