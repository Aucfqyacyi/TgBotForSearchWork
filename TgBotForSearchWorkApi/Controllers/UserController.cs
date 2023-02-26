using Deployf.Botf;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Repositories;

namespace TgBotForSearchWorkApi.Controllers;

public class UserController : BotController
{
    private readonly UserRepository _userRepository;

    public UserController(UserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [Action(Command.Start, CommandDescription.Start)]
    public async Task Start()
    {
        if(_userRepository.Create(ChatId, CancelToken))
            await Send("Вітання!");
        else
            await Send("Ваш обліковий запис вже створенно.");
    }

    [Action(Command.Stop, CommandDescription.Stop)]
    public async Task Stop()
    {
        _userRepository.Activate(ChatId, false, CancelToken);
        await Send("До побачення!");
    }

    [Action(Command.ChangeDescriptionLength, CommandDescription.ChangeDescriptionLength)]
    public async Task ChangeDescriptionLength()
    {              
        await State(new ChangingDescriptionLength());
        Push("Введіть бажаний розмір опису, діапазон значень від 0 до 6000.");
    }

    [State]
    public async Task HandleChangingDescriptionLength(ChangingDescriptionLength state)
    {
        await ClearState();
        if (int.TryParse(Context.GetSafeTextPayload(), out int descriptionLength))
        {
            _userRepository.UpdateDescriptionLength(ChatId, descriptionLength, CancelToken);
            await Send("Розмір опису змінено.");
        }
        else
        {
            await Send("Не корректне повідомлення.");
        }        
    }

    [Action(Command.Test)]
    public async Task Test()
    {
        await Send("Тестовий визов.");
    }
}
