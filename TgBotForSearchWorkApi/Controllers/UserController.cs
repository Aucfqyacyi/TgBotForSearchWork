using Deployf.Botf;
using TgBotForSearchWorkApi.Constants;

namespace TgBotForSearchWorkApi.Controllers;

public class UserController : BotController
{
    public UserController()
    {
    }

    [Action(Command.Start, CommandDescription.Empty)]
    public async Task Start()
    {
        await Send("Вітання!");
    }

    [Action(Command.Stop, CommandDescription.Empty)]
    public async Task Stop()
    {
        await Send("До побачення!");
    }

    [Action(Command.Test)]
    public async Task Test()
    {
        await Send("Тестовий визов.");
    }
}
