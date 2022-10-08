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
var urls = ReadAllUrlsFromFile();
var client = GHttpClient.Client;
var responseDou = await client.GetAsync(Host.Dou);
var responseDjinni = await client.GetAsync(Host.Djinni);
VacancyParser vacancyParserDou = new DouVacancyParser();
VacancyParser vacancyParserDjinni = new DjinniVacancyParser();
var vacsDou = await vacancyParserDou.ParseAsync(responseDou.Content.ReadAsStream());
var vacsDjinni = await vacancyParserDjinni.ParseAsync(responseDjinni.Content.ReadAsStream());
Console.WriteLine();

IEnumerable<Uri> ReadAllUrlsFromFile()
{
    using FileStream file = File.OpenRead("readyUrls.txt");
    using StreamReader streamReader = new(file);
    List<Uri> uris = new();
    string? line = null;
    while ((line = streamReader.ReadLine()) is not null)
    {
        uris.Add(new(line));
    }
    streamReader.Close();
    file.Close();
    return uris;
}
