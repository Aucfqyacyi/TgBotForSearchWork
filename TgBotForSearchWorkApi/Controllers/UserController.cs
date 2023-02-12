using Deployf.Botf;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Services;


namespace TgBotForSearchWorkApi.Controllers;

public class UserController : BotController
{
    private readonly UserRepository _userRepository;

    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
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
        _userRepository.AddDefaultUser();
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
