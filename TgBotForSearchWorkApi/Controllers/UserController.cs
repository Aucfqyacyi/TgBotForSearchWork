using Deployf.Botf;
using TgBotForSearchWork.Core;
using TgBotForSearchWork.Services;
using TgBotForSearchWorkApi.Constants;

namespace TgBotForSearchWorkApi.Controllers;

public class UserController : BaseController
{
    private readonly UserRepository _userRepository;

    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [Action(Command.Start, CommandDescription.Empty)]
    public void Start()
    {
        _userRepository.AddUser(ChatId, CancelToken);
        Send("Вітання!");
    }

    [Action(Command.Stop, CommandDescription.Empty)]
    public void Stop()
    {
        _userRepository.RemoveUser(ChatId, CancelToken);
        Send("До побачення!");
    }

    [Action(Command.Test, CommandDescription.Empty)]
    public void Test()
    {
        Send($"Тестовий визов.");
    }
}
