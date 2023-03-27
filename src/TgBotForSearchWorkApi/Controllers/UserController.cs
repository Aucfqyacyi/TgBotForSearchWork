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
        if (await _userRepository.CreateAsync(ChatId, CancelToken))
            await Send("Вітання!");
        else
            await Send("Ваш обліковий запис вже створенно.");
    }

    [Action(Command.Stop, CommandDescription.Stop)]
    public async Task Stop()
    {
        await _userRepository.ActivateAsync(ChatId, false, CancelToken);
        await Send("До побачення! Бот більше не буде присилати вам вакансії. Щоб заново активуватися викличте команду /start.");
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
        if (uint.TryParse(Context.GetSafeTextPayload(), out uint descriptionLength))
        {
            await _userRepository.UpdateAsync(ChatId, descriptionLength, CancelToken);
            await Send("Розмір опису змінено.");
        }
        else
        {
            await State(new ChangingDescriptionLength());
            await Send("Не корректне повідомлення.");
        }
    }
}
