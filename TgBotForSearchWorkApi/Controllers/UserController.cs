using Deployf.Botf;
using MongoDB.Bson;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Services;

namespace TgBotForSearchWorkApi.Controllers;

public class UserController : BotController
{
    private readonly UserRepository _userRepository;
    private readonly DefaultUserService _defaultUserService;

    public UserController(UserRepository userRepository, DefaultUserService defaultUserService)
    {
        _userRepository = userRepository;
        _defaultUserService = defaultUserService;
    }

    [Action(Command.Start, CommandDescription.Empty)]
    public async Task Start()
    {
        _userRepository.Add(ChatId, CancelToken);
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
        _userRepository.Remove(ChatId, CancelToken);
        await Send("До побачення!");
    }

    [Action(Command.Test)]
    public async Task Test()
    {
        await Send($"Тестовий визов.");
    }
}
