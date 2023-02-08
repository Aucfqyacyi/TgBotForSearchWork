using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;
using User = TgBotForSearchWork.Models.User;

namespace TgBotForSearchWork.Core;

internal class CommandHandler
{
    private readonly MongoDbService _mongoDbService;
    private readonly UserService _userService;

    public CommandHandler(MongoDbService mongoDbService, UserService userService)
    {
        _mongoDbService = mongoDbService;
        _userService = userService;
    }

    public async Task OnMessageAsync(ITelegramBotClient telegramBotClient, Update update, CancellationToken cancellationToken)
    {
        string messageText = update.Message!.Text!;
        long chatId = update.Message!.Chat.Id;
        await OnMessageAsync(new(chatId, telegramBotClient, cancellationToken), messageText);
    }

    private async Task OnMessageAsync(TelegramEntity telegramEntity, string messageText)
    {
        if(messageText == Command.Start)
        {
            _mongoDbService.AddUser(telegramEntity.ChatId, telegramEntity.CancellationToken);
            return;
        }
        if (messageText == Command.Stop)
        {
            _mongoDbService.RemoveUser(telegramEntity.ChatId, telegramEntity.CancellationToken);
            return;
        }
        if (messageText == Command.GetAllUrls)
        {
            await OnGetAllUrlsAsync(telegramEntity);
            return;
        }
        if (messageText == Command.Test)
        {
            await OnTestCommandAsync(telegramEntity);
            return;
        }
        if (messageText.StartsWith(Command.AddUrl))
        {
            await OnAddUrlCommandAsync(telegramEntity, messageText);
            return;
        }
        await OnIncorrectCommandAsync(telegramEntity);
    }

    private Task OnAddUrlCommandAsync(TelegramEntity telegramEntity, string messageText)
    {
        string url = messageText.Remove(0, Command.AddUrl.Length + 1);
        _mongoDbService.AddUrlToUser(telegramEntity.ChatId, new(url), telegramEntity.CancellationToken);
        return telegramEntity.TelegramBotClient.SendTextMessageAsync(telegramEntity.ChatId, $"Тестовий визов.",
                                                        cancellationToken: telegramEntity.CancellationToken);
    }

    private async Task OnGetAllUrlsAsync(TelegramEntity telegramEntity)
    {
        User? user = _mongoDbService.GetUserOrDefault(telegramEntity.ChatId, telegramEntity.CancellationToken);
        if (user == null)
            return;
        foreach (var message in _userService.GetGroupedUrls(user))
        {
            await telegramEntity.TelegramBotClient.SendTextMessageAsync(telegramEntity.ChatId,
                                                          message,
                                                          disableWebPagePreview: true,
                                                          cancellationToken: telegramEntity.CancellationToken);
        }
    }

    private Task OnTestCommandAsync(TelegramEntity telegramEntity)
    {
        Log.Info($"Test command was called.");
        return telegramEntity.TelegramBotClient.SendTextMessageAsync(telegramEntity.ChatId, $"Тестовий визов.", 
                                                        cancellationToken: telegramEntity.CancellationToken);
    }

    private Task OnIncorrectCommandAsync(TelegramEntity telegramEntity)
    {
        return telegramEntity.TelegramBotClient.SendTextMessageAsync(telegramEntity.ChatId, "Будь ласка, виберіть команду з списку.", 
                                                        cancellationToken: telegramEntity.CancellationToken);
    }
}
