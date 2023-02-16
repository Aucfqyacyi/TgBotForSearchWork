using Deployf.Botf;
using MongoDB.Bson;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Services;

namespace TgBotForSearchWorkApi.Controllers;

public class UserController : BotController
{
    private readonly DefaultUserService _defaultUserService;

    public UserController(DefaultUserService defaultUserService)
    {
        _defaultUserService = defaultUserService;
    }

    [Action(Command.Start, CommandDescription.Empty)]
    public async Task Start()
    {
        await Send("Вітання!");
    }

    [Action("/adu")]
    public void AddDefaultUser()
    {
        _defaultUserService.AddDefaultUser();
    }

    [Action(Command.Stop, CommandDescription.Empty)]
    public async Task Stop()
    {
        await Send("До побачення!");
    }

    [Action(Command.Test)]
    public async Task Test()
    {
        await Send($"Тестовий визов.");
    }
}
