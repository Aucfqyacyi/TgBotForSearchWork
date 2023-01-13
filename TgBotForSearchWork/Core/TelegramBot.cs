using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWork.Constants;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;
using User = TgBotForSearchWork.Models.User;

namespace TgBotForSearchWork.Core;

public class TelegramBot
{
	private readonly TelegramBotClient _telegramBotClient;
    private readonly VacancyService _vacancyService = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ReceiverOptions _receiverOptions;
    private readonly UserService _userService;
    private readonly TimeSpan _timeOut;

    public TelegramBot(string token, TimeSpan timeOut, UserService userManager, ReceiverOptions? receiverOptions = null)
    {
        _telegramBotClient = new(token, GHttpClient.Client);
        receiverOptions ??= new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
        _receiverOptions = receiverOptions;
        _userService = userManager;
        _timeOut = timeOut;
    }

    public async Task StartAsync()
    {
        _telegramBotClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: _receiverOptions,
                    cancellationToken: _cancellationTokenSource.Token);

        while (_cancellationTokenSource.Token.IsCancellationRequested is false)
        {
            try
            {
                foreach (var user in _userService.GetAllUsers(_cancellationTokenSource.Token))
                    await SendVacancyAsync(user, _cancellationTokenSource.Token);
                await Task.Delay(_timeOut, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                Log.Info($"Task canceled, cancellation is requested in main token = {_cancellationTokenSource.Token.IsCancellationRequested}.");
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }            
    }

    private async Task SendVacancyAsync(User user, CancellationToken cancellationToken = default)
    {       
        await SendVacancyAsync(user.ChatId, await _vacancyService.GetRelevantVacancies(user, cancellationToken), cancellationToken);
        _userService.UpdateUser(user, cancellationToken);
    }

    private async Task SendVacancyAsync(long chatId, IReadOnlyList<Vacancy> vacancies, CancellationToken cancellationToken = default)
    {
        for (int i = 0; i < vacancies.Count; i++)
            await SendVacancyAsync(chatId, vacancies[i], cancellationToken);
    }

    private Task SendVacancyAsync(long chatId, Vacancy vacancy, CancellationToken cancellationToken = default)
    {
        return _telegramBotClient.SendTextMessageAsync(chatId, 
                                                      vacancy.Present(), 
                                                      ParseMode.Markdown,
                                                      disableWebPagePreview: true,
                                                      cancellationToken: cancellationToken);
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
            }       
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
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

        Log.Info(errorMessage);
        return Task.CompletedTask;
    }

    private async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        string messageText = update.Message!.Text!;
        long chatId = update.Message!.Chat.Id;
        if (messageText.Contains(Command.ShowAllUrls))
            await OnShowAllUrls(chatId, cancellationToken);
        if (messageText.Contains(Command.Start))
            _userService.AddUser(chatId, null, cancellationToken);
        if (messageText.Contains(Command.Stop))
            _userService.RemoveUser(chatId, cancellationToken);
        if (messageText.Contains(Command.Test))
        {
            await botClient.SendTextMessageAsync(chatId, "Hello, I am friendly neighborhood bot <3.", cancellationToken: cancellationToken);
            Log.Info($"Test command was called.");
        }          
    }

    private async Task OnShowAllUrls(long chatId, CancellationToken cancellationToken = default)
    {
        User? user = _userService.GetUserOrDefault(chatId, cancellationToken);
        if (user is not null)
        {
            foreach (UrlToVacancies url in user.Urls)
            {
                await _telegramBotClient.SendTextMessageAsync(chatId, 
                                                              url.OriginalString, 
                                                              disableWebPagePreview: true,
                                                              cancellationToken: cancellationToken);
            }
        }      
    }
}
