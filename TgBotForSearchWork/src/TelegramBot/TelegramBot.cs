using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWork.src.Other;

namespace TgBotForSearchWork.src.TelegramBot;

public class TelegramBot
{
	private readonly TelegramBotClient _telegramBotClient;
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    private readonly ReceiverOptions _receiverOptions;

    public TelegramBot(string token, ReceiverOptions? receiverOptions = null)
	{
		_telegramBotClient = new(token, GHttpClient.Client);
        receiverOptions ??= new ReceiverOptions { AllowedUpdates = Array.Empty<UpdateType>() };
        _receiverOptions = receiverOptions;
    }

    public async Task Start()
    {
        _telegramBotClient.StartReceiving(
                    updateHandler: HandleUpdateAsync,
                    pollingErrorHandler: HandlePollingErrorAsync,
                    receiverOptions: _receiverOptions,
                    cancellationToken: _cancellationTokenSource.Token);
        while (Console.ReadLine() != "111") await Task.Delay(100);
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken = default)
    {
        if (update.Message is not { } message)
            return;

        if (message.Text is not { } messageText)
            return;

        var chatId = message.Chat.Id;

        Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");

        Message sentMessage = await botClient.SendTextMessageAsync(
            chatId: chatId,
            text: "You said:\n" + messageText,
            cancellationToken: cancellationToken);
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
