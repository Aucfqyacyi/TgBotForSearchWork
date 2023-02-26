using Deployf.Botf;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models.States;
using TgBotForSearchWorkApi.Models;
using MongoDB.Bson;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    [Action(Command.AddUrl, CommandDescription.Empty)]
    public async Task AddUrl()
    {
        await State(new AddingUrlState());
        Push("Напишіть посилання, яке бажаєте додати, у повному форматі.");
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
