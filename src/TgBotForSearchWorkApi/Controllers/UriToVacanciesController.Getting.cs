using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{

    [Action(Command.GetUrl, CommandDescription.GetUrl)]
    public async Task GetUrl()
    {
        await ShowSitesThenShowUrisToVacancies(SendUriToVacanciesAsync);
    }

    [Action]
    private async Task ShowSitesThenShowUrisToVacancies(Delegate next)
    {
        IEnumerable<SiteType> siteTypes = (await _uriToVacanciesRepository.GetAllAsSiteTypesAsync(ChatId, CancelToken))
                                                                          .Distinct();
        if (siteTypes.Any())
            ShowSites(siteType => Q(ShowUrisToVacancies, 0, siteType, next), siteTypes);
        else
            Push("У вас немає жодної вакансій.");
    }

    [Action]
    private async Task ShowUrisToVacancies(int page, SiteType siteType, Delegate next)
    {
        List<UriToVacancies> urlsToVacancies = await _uriToVacanciesRepository.GetAllAsync(ChatId, siteType, CancelToken);
        if (urlsToVacancies.Any() is false)
        {
            await ShowSitesThenShowUrisToVacancies(next);
            return;
        }
        Push("Виберіть потрібне посилання.");
        Pager(urlsToVacancies, page, indexToUrl => (indexToUrl.WithoutHttps, Q(next, indexToUrl.Id, siteType)),
                                        Q(ShowUrisToVacancies, _firstPage, siteType, next), 1);
        RowButton(_back, Q(ShowSitesThenShowUrisToVacancies, next));
    }

    [Action]
    private async Task SendUriToVacanciesAsync(ObjectId urlId, SiteType siteType)
    {
        UriToVacancies uriToVacancies = await _uriToVacanciesRepository.GetAsync(urlId, CancelToken);
        ActivateRowButton(urlId, uriToVacancies.IsActivated);
        await Send(uriToVacancies.OriginalString, new() { DisableWebPagePreview = true });
    }
}
