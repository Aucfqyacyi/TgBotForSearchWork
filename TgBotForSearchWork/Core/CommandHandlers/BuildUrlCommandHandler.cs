using Parsers.Constants;
using Parsers.Models;
using Telegram.Bot;
using TgBotForSearchWork.Constants;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Services;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWork.Core.CommandHandlers;

internal class BuildUrlCommandHandler
{
    private readonly FilterService _filterService;

    public BuildUrlCommandHandler(FilterService filterService)
    {
        _filterService = filterService;
    }

    public Task OnBuildUrlAsync(TelegramEntity telegramEntity)
    {
        var keyboard = ResizedKeyboardMarkup.MakeList(Enum.GetNames<SiteType>(), KeyboardButtonPrefix.WhenBuildingUrl);
        return OnBuildUrlAsync(telegramEntity, keyboard);
    }

    public Task OnBuildingUrlAsync(TelegramEntity telegramEntity, string messageText)
    {
        int prefixCount = messageText.Count(e => e == KeyboardButtonPrefix.WhenBuildingUrl);
        string textWithoutPrefix = messageText.Substring(messageText.IndexOf(' ') + 1);
        switch (prefixCount)
        {
            case 1:
                return OnBuildUrlAsync(telegramEntity, GetKeyboardCategories(prefixCount, textWithoutPrefix));
            case 2:
                return OnBuildUrlAsync(telegramEntity, GetKeyboardFilters(prefixCount, messageText, textWithoutPrefix));
            case 3:
                break;
        }
        return Task.CompletedTask;
    }

    private ResizedKeyboardMarkup GetKeyboardCategories(int prefixCount, string textWithoutPrefix)
    {
        SiteType siteType = Enum.Parse<SiteType>(textWithoutPrefix);
        List<string> categories = _filterService.GetFilterCategories(siteType);
        return ResizedKeyboardMarkup.MakeList(categories, KeyboardButtonPrefix.WhenBuildingUrl, prefixCount + 1, (int)siteType);
    }

    private ResizedKeyboardMarkup GetKeyboardFilters(int prefixCount, string messageText, string textWithoutPrefix)
    {
        SiteType siteType = (SiteType)(messageText[prefixCount] - 48);
        IEnumerable<Filter> filters = _filterService.SiteTypeToFilters[siteType]
                                         .Where(filter => filter.CategoryName == textWithoutPrefix);
        return ResizedKeyboardMarkup.MakeList(filters, KeyboardButtonPrefix.WhenBuildingUrl, prefixCount + 1, (int)siteType);
    }

    private Task OnBuildUrlAsync(TelegramEntity telegramEntity, ResizedKeyboardMarkup keyboard)
    {
        return telegramEntity.TelegramBotClient.SendTextMessageAsync(telegramEntity.ChatId,
                                                      "Зробить вибір.",
                                                      disableWebPagePreview: true,
                                                      replyMarkup: keyboard,
                                                      cancellationToken: telegramEntity.CancellationToken);
    }
}
