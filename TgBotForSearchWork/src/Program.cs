using AngleSharp.Dom;
using AngleSharp;
using TgBotForSearchWork.src.Other;
using TgBotForSearchWork.src.TelegramBot;
using AngleSharp.Io;
using System.Text;
using TgBotForSearchWork.src.VacancyParser;


/*TelegramBot telegramBot = new("2128837514:AAEJVAT5VUFQcXGo03tsCj0KobKns8HQRgU");
await telegramBot.Start();
telegramBot.Stop();*/

Console.OutputEncoding = Encoding.Unicode;
var client = GHttpClient.Client;
var responseDou = await client.GetAsync(Host.Dou);
var responseDjinni = await client.GetAsync(Host.Djinni);
BaseVacancyParser vacancyParserDou = new DouVacancyParser();
BaseVacancyParser vacancyParserDjinni = new DjinniVacancyParser();
var vacsDou = await vacancyParserDou.ParseAsync(responseDou.Content.ReadAsStream(), Host.Dou.OriginalString);
var vacsDjinni = await vacancyParserDjinni.ParseAsync(responseDjinni.Content.ReadAsStream(), Host.TempDjinni.OriginalString);
Console.WriteLine();