using System.Text;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotForSearchWork.src.Extensions;
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
        catch (TaskCanceledException)
        {

        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Stop();
        }      
    }

    private async Task SendVacancy(User user, CancellationToken cancellationToken = default)
    {
        foreach (var uri in user.HashsToUris.Values)
        {
            using Stream response = await GHttpClient.GetAsync(uri, cancellationToken);
            List<Vacancy> vacancies = await GetRelevantVacancies(response, uri, user.LastVacancy, cancellationToken);
            if (vacancies.Count != 0)
            {
                await SendVacancy(user.ChatId, vacancies);
                user.LastVacancy = vacancies.FirstOrDefault();
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
        await _telegramBotClient.SendTextMessageAsync(chatId, 
                                                      vacancy.Present(), 
                                                      ParseMode.Markdown,
                                                      replyMarkup: GetInlineButton(vacancy.Url),
                                                      disableWebPagePreview: true,
                                                      cancellationToken: cancellationToken);
    }

    private async Task<List<Vacancy>> GetRelevantVacancies(Stream response, Uri uri, Vacancy? lastVacancy, 
                                                                            CancellationToken cancellationToken = default)
    {
        VacancyParser vacancyParser = VacancyParserFactory.Create(uri);
        List<Vacancy> vacancies = await vacancyParser.ParseAsync(response, uri.Host, cancellationToken);
        if (lastVacancy is not null)
        {
            if (lastVacancy == vacancies.FirstOrDefault())
            {
                return new();
            }
            int index = vacancies.FindIndex(vacancy => vacancy.Title == lastVacancy.Title);
            if (index != -1)
            {
                vacancies.RemoveRange(index, vacancies.Count - index);
            }
        }
        return vacancies;
    }

    public void Stop()
    {
        if (_cancellationTokenSource.IsCancellationRequested is false)
        {
            _cancellationTokenSource.Cancel();
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {      
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await OnMessage(botClient, update, cancellationToken);
                    break;
                case UpdateType.CallbackQuery:
                    await OnCallbackQuery(botClient, update, cancellationToken);
                    break;
            }       
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

    private IReplyMarkup GetInlineButton(string url)
    {
        return new InlineKeyboardMarkup(new InlineKeyboardButton(Command.GetFullVacancy) { CallbackData = url.GetMD5() });
    }

    private async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        string messageText = update.Message!.Text!;
        var chatId = update.Message!.Chat.Id;
        if (Command.Start.Contains(messageText))
            _userManager.AddUser(chatId);
        if (Command.Stop.Contains(messageText))
            _userManager.RemoveUser(chatId);
        if (Command.Test.Contains(messageText))
            await botClient.SendTextMessageAsync(chatId, "Hello, I am friendly neighborhood bot <3.\n", cancellationToken: cancellationToken);
    }

    private async Task OnCallbackQuery(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        string data = update.CallbackQuery!.Data!;
        var chatId = update.CallbackQuery!.Message!.Chat.Id;
        await botClient.SendTextMessageAsync(chatId, data, cancellationToken: cancellationToken);
    }
}
