using Telegram.Bot;

namespace TgBotForSearchWork.Models;

internal record TelegramEntity
{
    public long ChatId { get; set; }
    public ITelegramBotClient TelegramBotClient { get; set; }
    public CancellationToken CancellationToken { get; set; }

    public TelegramEntity(long chatId, ITelegramBotClient telegramBotClient, CancellationToken cancellationToken)
    {
        ChatId = chatId;
        TelegramBotClient = telegramBotClient;
        CancellationToken = cancellationToken;
    }
}
