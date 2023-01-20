using Parsers.Models;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
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
                foreach (var user in await _userService.GetAllUsersAsync(_cancellationTokenSource.Token))
                    await SendVacancyAsync(user, _cancellationTokenSource.Token);
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
        Log.Info($"All vacancies to user({user.ChatId}) were sent successfully.");
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
                    await OnMessage(botClient, update, cancellationToken);
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

    private async Task OnMessage(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        string messageText = update.Message!.Text!;
        long chatId = update.Message!.Chat.Id;
        if (messageText.Contains(Command.ShowAllUrls))
            await OnShowAllUrls(chatId, cancellationToken);
        if (messageText.Contains(Command.Start))
            await _userService.AddUserAsync(chatId, cancellationToken);
        if (messageText.Contains(Command.Stop))
            await _userService.RemoveUserAsync(chatId, cancellationToken);
        if (messageText.Contains(Command.Test))
        {
            await botClient.SendTextMessageAsync(chatId, "Hello, I am friendly neighborhood bot <3.", cancellationToken: cancellationToken);
            Log.Info($"Test command was called.");
        }          
    }

    private async Task OnShowAllUrls(long chatId, CancellationToken cancellationToken)
    {
        User? user = await _userService.GetUserOrDefaultAsync(chatId, cancellationToken);
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
