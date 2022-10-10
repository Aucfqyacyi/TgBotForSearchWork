using TgBotForSearchWork.Core;
using TgBotForSearchWork.Core.UserManagers;
using TgBotForSearchWork.Core.FileManagers;



FileManager fileManager = new("Users.txt");
UserManager userManager = new(fileManager);
TelegramBot telegramBot = new("2128837514:AAEJVAT5VUFQcXGo03tsCj0KobKns8HQRgU", TimeSpan.FromSeconds(15), userManager);
telegramBot.Start();
while (Console.ReadLine() != "111") ;
telegramBot.Stop();

