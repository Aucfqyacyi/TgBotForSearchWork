using Deployf.Botf;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UrlToVacanciesController
{
    [Action(Command.AddUrl, CommandDescription.Empty)]
    public void AddUrl()
    {
        State(new AddingUrlState());
        Push("Напишіть посилання, яке бажаєте додати, у повному форматі.");
    }

    [State]
    private async Task HandleAddingUrlAsync(AddingUrlState state)
    {
        UrlToVacancies? urlToVacancies = await _userService.AddUrlToVacancyAsync(ChatId, Context.GetSafeTextPayload()!, CancelToken);
        ActivateRowButton(urlToVacancies?.Id, urlToVacancies?.IsActivate);
        if (urlToVacancies is not null)
            Push("Посилання було добавленно.");
        else
            Push("Посилання не корректне, або не містить жодної вакансії, або вже добавленно.");
    }
}
