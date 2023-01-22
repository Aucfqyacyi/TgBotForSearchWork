using Parsers.Constants;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;
using User = TgBotForSearchWork.Models.User;

namespace TgBotForSearchWork.Core;

internal class CommandHandler
{
    private readonly ITelegramBotClient _telegramBotClient;
    private readonly UserService _userService;

    public CommandHandler(ITelegramBotClient telegramBotClient, UserService userService)
    {
        _userService = userService;
        _telegramBotClient = telegramBotClient;
    }

    public async Task OnMessageAsync(Update update, CancellationToken cancellationToken)
    {
        string messageText = update.Message!.Text!;
        long chatId = update.Message!.Chat.Id;
        await OnMessageAsync(chatId, messageText, cancellationToken);
    }

    private async Task OnMessageAsync(long chatId, string messageText, CancellationToken cancellationToken)
    {
        bool wasIncorrectCommand = false;
        switch (messageText)
        {
            case Command.Start:
                _userService.AddUser(chatId, cancellationToken);
                break;
            case Command.Stop:
                _userService.RemoveUser(chatId, cancellationToken);
                break;
            case Command.ShowAllUrls:
                await OnShowAllUrlsAsync(chatId, cancellationToken);
                break;
            case Command.GetAllUrls:
                await OnGetAllUrlsAsync(chatId, cancellationToken);
                break;
            case Command.BuildUrl:
                await OnBuildUrlAsync(chatId, cancellationToken);
                break;
            case Command.Test:
                await OnTestCommandAsync(chatId, cancellationToken);
                break;
            default:
                wasIncorrectCommand = true;
                break;
        }

        if (SiteTypesToUris.TheirHosts.Values.Contains(messageText))
            OnChooseHostDuringBuildingUrl(messageText);

        if (!wasIncorrectCommand)
            _userService.UpdateLastCommandFieldInUser(chatId, messageText, cancellationToken);
        else
            await OnIncorrectCommandAsync(chatId, cancellationToken);
    }

    private async Task OnGetAllUrlsAsync(long chatId, CancellationToken cancellationToken)
    {
        foreach (var message in _userService.GetGroupedUrls(chatId, cancellationToken))
        {
            await _telegramBotClient.SendTextMessageAsync(chatId,
                                                          message,
                                                          disableWebPagePreview: true,
                                                          cancellationToken: cancellationToken);
        }
    }

    private void OnChooseHostDuringBuildingUrl(string host)
    {


    }

    private Task OnBuildUrlAsync(long chatId, CancellationToken cancellationToken)
    {
        return OnBuildUrlAsync(chatId, ResizedReplyKeyboardMarkup.MakeList(SiteTypesToUris.TheirHosts.Values), cancellationToken);
    }

    private Task OnBuildUrlAsync(long chatId, ResizedReplyKeyboardMarkup keyboard, CancellationToken cancellationToken)
    {
        return _telegramBotClient.SendTextMessageAsync(chatId,
                                                      "Зробить вибір.",
                                                      disableWebPagePreview: true,
                                                      replyMarkup: keyboard,
                                                      cancellationToken: cancellationToken);
    }

    private async Task OnShowAllUrlsAsync(long chatId, CancellationToken cancellationToken)
    {
        User? user = _userService.GetUserOrDefault(chatId, cancellationToken);
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

    private Task OnTestCommandAsync(long chatId, CancellationToken cancellationToken)
    {
        Log.Info($"Test command was called.");
        return _telegramBotClient.SendTextMessageAsync(chatId, "Тестовий визов.", cancellationToken: cancellationToken);
    }

    private Task OnIncorrectCommandAsync(long chatId, CancellationToken cancellationToken)
    {
        return _telegramBotClient.SendTextMessageAsync(chatId, "Будь ласка, виберіть команду з списку.", cancellationToken: cancellationToken);
    }
}
