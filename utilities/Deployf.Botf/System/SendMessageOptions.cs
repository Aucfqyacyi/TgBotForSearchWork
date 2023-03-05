using Telegram.Bot.Types.Enums;

namespace Deployf.Botf.System;

public class SendMessageOptions
{
    public bool? DisableWebPagePreview { get; set; }
    public bool? DisableNotification { get; set; }
    public bool? ProtectCotent { get; set; }
    public bool? AllowSendingWithoutReply { get; set; }
    public ParseMode ParseMode { get; set; } = ParseMode.Html;
}
