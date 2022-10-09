using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWork.src.Other;
using TgBotForSearchWork.src.TelegramBot.FileManagers;
using TgBotForSearchWork.src.TelegramBot.Models;
using TgBotForSearchWork.src.VacancyParsers;
using User = TgBotForSearchWork.src.TelegramBot.Models.User;

namespace TgBotForSearchWork.src.TelegramBot;

public class TelegramBot
{
	private readonly TelegramBotClient _telegramBotClient;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ReceiverOptions _receiverOptions;
    private readonly UserManager _userManager;

    public TelegramBot(string token, UserManager userManager, ReceiverOptions ? receiverOptions = null)
    {
        _telegramBotClient = new(token, GHttpClient.Client);
        receiverOptions ??= new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
        _receiverOptions = receiverOptions;
        _userManager = userManager;
    }

    public async Task Start()
    {
        _telegramBotClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: _receiverOptions,
                    cancellationToken: _cancellationTokenSource.Token);
        try
        {
            while (true)
            {
                foreach (var user in _userManager.Users)
                    await SendVacancy(user, _cancellationTokenSource.Token);
                await Task.Delay(TimeSpan.FromMinutes(3), _cancellationTokenSource.Token);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            _cancellationTokenSource.Cancel();
        }      
    }

    private async Task SendVacancy(User user, CancellationToken cancellationToken = default)
    {
        foreach (var uriToVacancy in user.UrisToVacancies)
        {
            using Stream response = await GHttpClient.GetAsync(uriToVacancy.Key, cancellationToken);
            List<Vacancy> vacancies = await GetRelevantVacancies(response, uriToVacancy, cancellationToken);
            if (vacancies.Count != 0)
            {
                await SendVacancy(user.ChatId, vacancies);
                user.UrisToVacancies[uriToVacancy.Key] = vacancies.FirstOrDefault();
            }
        }
    }

    private async Task SendVacancy(long chatId, List<Vacancy> vacancies, CancellationToken cancellationToken = default)
    {
        foreach (var vacancy in vacancies)
            await SendVacancy(chatId, vacancy, cancellationToken);
    }

    private async Task SendVacancy(long chatId, Vacancy vacancy, CancellationToken cancellationToken = default)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId, vacancy.Present(), ParseMode.Markdown,
                                                                                disableWebPagePreview: true,
                                                                                cancellationToken: cancellationToken);
    }

    private async Task<List<Vacancy>> GetRelevantVacancies(Stream response, KeyValuePair<Uri, Vacancy?> uriToVacancy, 
                                                                            CancellationToken cancellationToken = default)
    {
        VacancyParser vacancyParser = VacancyParserFactory.Create(uriToVacancy.Key);
        List<Vacancy> vacancies = await vacancyParser.ParseAsync(response, uriToVacancy.Key.Host, cancellationToken);
        if (uriToVacancy.Value is not null)
        {
            if (uriToVacancy.Value == vacancies.FirstOrDefault())
            {
                return new();
            }
            int index = vacancies.FindIndex(vacancy => vacancy.Title == uriToVacancy.Value.Title);
            if (index != -1)
            {
                vacancies.RemoveRange(index, vacancies.Count - index);
            }
        }
        return vacancies;
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message is not { } message || message.Text is not { } messageText)
            return;
        try
        {
            var chatId = message.Chat.Id;
            if (Command.Start.Contains(messageText))
                _userManager.AddUser(chatId);
            if (Command.Stop.Contains(messageText))
                _userManager.RemoveUser(chatId);
            if (Command.Test.Contains(messageText))
                await botClient.SendTextMessageAsync(chatId, "Hello, I am friendly neighborhood bot <3.\n", cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken = default)
    {
        string errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(errorMessage);
        return Task.CompletedTask;
    }
  
}
