using Deployf.Botf;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWorkApi.Controllers;

public class BaseController : BotController
{
    protected long _lastExceptionMessageId = 0;
    protected long _lastUnknownMessageId = 0;

    [On(Handle.Unknown)]
    public void OnUnknownCommand()
    {
        int messageId = Context.Update.Id;
        if(messageId != _lastUnknownMessageId)
        {
            _lastUnknownMessageId = messageId;
            Send($"Будь ласка, виберіть команду з списку.");
        }       
    }

    [On(Handle.Exception)]
    public Task OnExceptionAsync(Exception exception)
    {
        int messageId = Context.Update.Id;
        if (messageId != _lastExceptionMessageId)
        {
            _lastExceptionMessageId = messageId;
            try
            {
                Log.Info($"Unhandled exception - {exception.Message}");
                if (Context.Update.Type == UpdateType.CallbackQuery)
                {
                    return AnswerCallback("Error");
                }
                ClearMessage();
                Send($"Вибачте, сталася помилка.");
            }
            catch (Exception ex)
            {
                Log.Info($"Exception in {nameof(OnExceptionAsync)} method - {ex.Message}");
            }
            
        }           
        return Task.CompletedTask;
    }
}
