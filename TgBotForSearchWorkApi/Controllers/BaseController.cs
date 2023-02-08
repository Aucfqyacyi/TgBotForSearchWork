using Deployf.Botf;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWorkApi.Controllers;

public class BaseController : BotController
{
    protected long _lastMessageId = 0;

    [On(Handle.Unknown)]
    public void OnUnknownCommand()
    {
        int messageId = Context.Update.Id;
        if(messageId != _lastMessageId)
        {
            _lastMessageId = messageId;
            Send($"Будь ласка, виберіть команду з списку.");
        }       
    }

    [On(Handle.Exception)]
    public Task OnExceptionAsync(Exception exception)
    {
        Log.Info($"Unhandled exception - {exception.Message}");
        if (Context.Update.Type == UpdateType.CallbackQuery)
        {
            return AnswerCallback("Error");
        }
        ClearMessage();
        Send($"Вибачте, сталася помилка.");
        return Task.CompletedTask;
    }
}
