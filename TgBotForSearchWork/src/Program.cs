using TgBotForSearchWork.src.TelegramBot;
using TgBotForSearchWork.src.TelegramBot.FileManagers;



FileManager fileManager = new("Users.txt");
UserManager userManager = new(fileManager);
TelegramBot telegramBot = new("2128837514:AAEJVAT5VUFQcXGo03tsCj0KobKns8HQRgU", userManager);
telegramBot.Start();
while (Console.ReadLine() != "111") ;
telegramBot.Stop();

