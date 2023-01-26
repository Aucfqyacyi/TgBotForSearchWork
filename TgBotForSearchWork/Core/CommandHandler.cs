using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotForSearchWork.Constants;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;
using User = TgBotForSearchWork.Models.User;

namespace TgBotForSearchWork.Core;

internal class CommandHandler
{
    private readonly UserService _userService;

    public CommandHandler(UserService userService)
    {
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
        switch (messageText)
        {
            case Command.Start:
                _userService.AddUser(telegramEntity.ChatId, telegramEntity.CancellationToken);
                break;
            case Command.Stop:
                _userService.RemoveUser(telegramEntity.ChatId, telegramEntity.CancellationToken);
                break;
            case Command.ShowAllUrls:
                await OnShowAllUrlsAsync(telegramEntity);
                break;
            case Command.GetAllUrls:
                await OnGetAllUrlsAsync(telegramEntity);
                break;
            /*case Command.BuildUrl:
                await _buildUrlCommandHandler.OnBuildUrlAsync(telegramEntity);
                break;*/
            case Command.Test:
                await OnTestCommandAsync(telegramEntity);
                break;
            default:
/*                if (messageText.StartsWith(KeyboardButtonPrefix.WhenBuildingUrl))
                    await _buildUrlCommandHandler.OnBuildingUrlAsync(telegramEntity, messageText);
                else*/
                    await OnIncorrectCommandAsync(telegramEntity);
                break;
        }      
    }

    private async Task OnGetAllUrlsAsync(TelegramEntity telegramEntity)
    {
        foreach (var message in _userService.GetGroupedUrls(telegramEntity.ChatId, telegramEntity.CancellationToken))
        {
            await telegramEntity.TelegramBotClient.SendTextMessageAsync(telegramEntity.ChatId,
                                                          message,
                                                          disableWebPagePreview: true,
                                                          cancellationToken: telegramEntity.CancellationToken);
        }
    }

    private async Task OnShowAllUrlsAsync(TelegramEntity telegramEntity)
    {
        User? user = _userService.GetUserOrDefault(telegramEntity.ChatId, telegramEntity.CancellationToken);
        if (user == null)
            return;
        List<List<KeyboardButton>> keyboardButtons = new List<List<KeyboardButton>>();
        for (int i = 0; i < user.Urls.Count; i++)
        {
            if (i % 2 == 0)
                keyboardButtons.Add(new List<KeyboardButton>());
            keyboardButtons.Last().Add(new(user.Urls[i].WithOutHttps));
        }

        await telegramEntity.TelegramBotClient.SendTextMessageAsync(telegramEntity.ChatId,
                                                      "Ваші посилання",
                                                      replyMarkup: new ResizedKeyboardMarkup(keyboardButtons),
                                                      cancellationToken: telegramEntity.CancellationToken);
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
