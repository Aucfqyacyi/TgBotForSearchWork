using Deployf.Botf;
using Telegram.Bot.Types.Enums;
using TgBotForSearchWorkApi.Utilities;


namespace TgBotForSearchWorkApi.Controllers;

public class MainController : BotController
{

    [On(Handle.Unknown)]
    public async Task OnUnknownCommand()
    {
        await Send($"Будь ласка, виберіть команду з списку.");
    }

    [On(Handle.Exception)]
    public async Task OnExceptionAsync(Exception exception)
    {
        try
        {
            Log.Info($"Unhandled exception - {exception.Message}");
            if (Context.Update.Type == UpdateType.CallbackQuery)
            {
                await AnswerCallback("Error");
            }
            ClearMessage();
            await Send($"Вибачте, сталася помилка.");
        }
        catch (Exception ex)
        {
            Log.Info($"Exception in {nameof(OnExceptionAsync)} method - {ex.Message}");
        }
    }
}
