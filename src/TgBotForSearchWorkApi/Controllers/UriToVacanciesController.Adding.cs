using Deployf.Botf;
using MongoDB.Bson;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Models.States;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    [Action(Command.AddUrl, CommandDescription.AddUrl)]
    public async Task AddUrl()
    {
        await State(new AddingUrlState());
        Push("Напишіть посилання у повному форматі, яке бажаєте додати. Наприклад посилання з сайту де ви вабрали усі потрібні фільтри.");
    }

    [State]
    private async Task HandleAddingUrlAsync(AddingUrlState state)
    {
        await ClearState();
        UriToVacancies? uriToVacancies = await _uriToVacanciesService.AddAsync(ChatId, Context.GetSafeTextPayload()!, CancelToken);
        OnAdd(uriToVacancies?.Id, uriToVacancies?.IsActivated);
    }

    [Action]
    private void OnAdd(ObjectId? uriId, bool? isActivated)
    {
        ActivateRowButton(uriId, isActivated, OnAdd);
        if (uriId is not null)
            Push("Посилання було добавленно.");
        else
            Push("Посилання не корректне, або не містить жодної вакансії, або вже добавленно.");
    }
}
