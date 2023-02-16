using Deployf.Botf;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
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
        UriToVacancies? uriToVacancies = await _uriToVacanciesService.AddAsync(ChatId, Context.GetSafeTextPayload()!, CancelToken);
        ActivateRowButton(uriToVacancies?.Id, uriToVacancies?.IsActivated);
        if (uriToVacancies is not null)
            Push("Посилання було добавленно.");
        else
            Push("Посилання не корректне, або не містить жодної вакансії, або вже добавленно.");
    }
}
