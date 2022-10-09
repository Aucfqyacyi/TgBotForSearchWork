using System.Text;
using TgBotForSearchWork.src.TelegramBot;
using TgBotForSearchWork.src.TelegramBot.FileManagers;





Console.OutputEncoding = Encoding.Unicode;
FileManager fileManager = new("Users.txt");
TelegramBot telegramBot = new("2128837514:AAEJVAT5VUFQcXGo03tsCj0KobKns8HQRgU", fileManager);
telegramBot.Start();
while (Console.ReadLine() != "111") ;
telegramBot.Stop();
