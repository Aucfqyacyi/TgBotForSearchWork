using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWork.Core;

internal class TelegramBot
{
	private readonly TelegramBotClient _telegramBotClient;  
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ReceiverOptions _receiverOptions;
    private readonly TimeSpan _timeOut;
    private readonly CommandHandler _commandHandler;
    private readonly VacancySender _vacancySender;

    public TelegramBot(string token, TimeSpan timeOut, VacancySender vacancySender, 
                        CommandHandler commandHandler, ReceiverOptions ? receiverOptions = null)
    {
        _telegramBotClient = new(token, GlobalHttpClient.Client);
        _receiverOptions = receiverOptions ?? new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
        _timeOut = timeOut;
        _vacancySender = vacancySender;
        _commandHandler = commandHandler;
    }

    private void PrepareAsync()
    {
        try
        {            
            _telegramBotClient.StartReceiving(
                        updateHandler: HandleUpdateAsync,
                        pollingErrorHandler: HandlePollingErrorAsync,
                        receiverOptions: _receiverOptions,
                        cancellationToken: _cancellationTokenSource.Token);
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
    }

    public async Task StartAsync()
    {
        PrepareAsync();
        while (_cancellationTokenSource.Token.IsCancellationRequested is false)
        {
            await _vacancySender.SendVacancyAsync(_telegramBotClient, _timeOut, _cancellationTokenSource.Token);
        }            
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
            _telegramBotClient.CloseAsync().GetAwaiter().GetResult();
        }
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {      
        try
        {
            switch (update.Type)
            {
                case UpdateType.Message:
                    await _commandHandler.OnMessageAsync(botClient, update, cancellationToken);
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
}
