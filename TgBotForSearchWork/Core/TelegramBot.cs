using AngleSharp.Dom;
using Parsers.Models;
using System;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;
using User = TgBotForSearchWork.Models.User;

namespace TgBotForSearchWork.Core;

internal class TelegramBot
{
	private readonly TelegramBotClient _telegramBotClient;
    private readonly VacancyService _vacancyService = new();
    private readonly FilterService _filterService = new();
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ReceiverOptions _receiverOptions;
    private readonly UserService _userService = new();
    private readonly TimeSpan _timeOut;

    public TelegramBot(string token, TimeSpan timeOut, ReceiverOptions? receiverOptions = null)
    {
        _telegramBotClient = new(token, GlobalHttpClient.Client);
        receiverOptions ??= new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
        _receiverOptions = receiverOptions;
        _timeOut = timeOut;
    }

    public async Task StartAsync()
    {
        await PrepareAsync();
        while (_cancellationTokenSource.Token.IsCancellationRequested is false)
        {
            try
            {
                /*foreach (var user in await _userService.GetAllUsersAsync(_cancellationTokenSource.Token))
                    await SendVacancyAsync(user, _cancellationTokenSource.Token);*/
                await Task.Delay(_timeOut, _cancellationTokenSource.Token);
            }
            catch (TaskCanceledException)
            {
                Log.Info($"Task canceled, IsCancellationRequested in main token = {_cancellationTokenSource.Token.IsCancellationRequested}.");
            }
            catch (Exception ex)
            {
                Log.Info(ex.Message);
            }
        }            
    }

    private async Task PrepareAsync()
    {
        _userService.AddDefaultUser();
        await _filterService.CollectFiltersAsync(_cancellationTokenSource.Token);
        _telegramBotClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: _receiverOptions,
                    cancellationToken: _cancellationTokenSource.Token);
    }

    private async Task SendVacancyAsync(User user, CancellationToken cancellationToken)
    {
        List<Vacancy> relevantVacancies = await _vacancyService.GetRelevantVacancies(user, cancellationToken);
        await _userService.UpdateUserAsync(user, cancellationToken);
        await SendVacancyAsync(user.ChatId, relevantVacancies, cancellationToken);
        Log.Info($"All vacancies for the user({user.ChatId}) were sent successfully.");
    }

    private async Task SendVacancyAsync(long chatId, IReadOnlyList<Vacancy> vacancies, CancellationToken cancellationToken)
    {
        for (int i = 0; i < vacancies.Count; i++)
        {
            try
            {
                await SendVacancyAsync(chatId, vacancies[i], cancellationToken);
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Log.Info($"Exception was thrown at vacancy({vacancies[i].Url})#{i},error - {ex.Message}");
            }
        }            
    }

    private Task SendVacancyAsync(long chatId, Vacancy vacancy, CancellationToken cancellationToken)
    {
        return _telegramBotClient.SendTextMessageAsync(chatId, 
                                                      vacancy.Present(), 
                                                      ParseMode.Markdown,
                                                      disableWebPagePreview: true,
                                                      cancellationToken: cancellationToken);
    }

    public void StopEvent(object? sender, ConsoleCancelEventArgs e)
    {
        Stop();
    }

    public void Stop()
    {
        if (_cancellationTokenSource.IsCancellationRequested is false)
        {     
            _cancellationTokenSource.Cancel();
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {      
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await OnMessageAsync(update, cancellationToken);
                    break;
            }       
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
    }

    private Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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

    private async Task OnMessageAsync(Update update, CancellationToken cancellationToken)
    {
        string messageText = update.Message!.Text!;
        long chatId = update.Message!.Chat.Id;
        await OnMessageAsync(chatId, messageText, cancellationToken);
    }

    private async Task OnMessageAsync(long chatId, string messageText, CancellationToken cancellationToken)
    {
        switch (messageText)
        {
            
            case Command.Start:
                await _userService.AddUserAsync(chatId, cancellationToken);
                break;
            case Command.Stop:
                await _userService.RemoveUserAsync(chatId, cancellationToken);
                break;
            case Command.ShowAllUrls:
                await OnShowAllUrlsAsync(chatId, cancellationToken);
                break;
            case Command.GetAllUrls:
                await OnGetAllUrlsAsync(chatId, cancellationToken);
                break;
            case Command.Test:
                await OnTestCommandAsync(chatId, cancellationToken);
                break;
            default:
                await OnIncorrectCommandAsync(chatId, cancellationToken);
                break;
        }
    }

    private async Task OnGetAllUrlsAsync(long chatId, CancellationToken cancellationToken)
    {
        foreach (var message in await _userService.GetGroupedUrlsAsync(chatId, cancellationToken))
        {
            await _telegramBotClient.SendTextMessageAsync(chatId,
                                                          message,
                                                          disableWebPagePreview: true,
                                                          cancellationToken: cancellationToken);
        }    
    }

    private async Task OnShowAllUrlsAsync(long chatId, CancellationToken cancellationToken)
    {
        User? user = await _userService.GetUserOrDefaultAsync(chatId, cancellationToken);
        if (user == null)
            return;
        List<List<KeyboardButton>> keyboardButtons = new List<List<KeyboardButton>>();
        for (int i = 0; i < user.Urls.Count; i++)
        {
            if (i % 2 == 0)
                keyboardButtons.Add(new List<KeyboardButton>());
            keyboardButtons.Last().Add(new(user.Urls[i].WithOutHttps));
        }

        await _telegramBotClient.SendTextMessageAsync(chatId,
                                                      "Ваші посилання",
                                                      replyMarkup: new ResizedReplyKeyboardMarkup(keyboardButtons),
                                                      cancellationToken: cancellationToken);
    }

    private async Task OnTestCommandAsync(long chatId, CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId, "Тестовий визов.", cancellationToken: cancellationToken);
        Log.Info($"Test command was called.");
    }

    private async Task OnIncorrectCommandAsync(long chatId, CancellationToken cancellationToken)
    {
        await _telegramBotClient.SendTextMessageAsync(chatId, "Будь ласка, виберіть команду з списку.", cancellationToken: cancellationToken);
    }
}
