using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{

    [Action(Command.GetUrl, CommandDescription.GetUrl)]
    public void GetUrl()
    {
        ShowSitesThenShowUrisToVacancies(GetUriToVacanciesAsync);
    }

    [Action]
    protected void ShowSitesThenShowUrisToVacancies(Delegate next)
    {
        ShowSites(siteType => Q(ShowUrisToVacancies, 0, siteType, next));
    }

    [Action]
    protected void ShowUrisToVacancies(int page, SiteType siteType, Delegate next)
    {
        List<UriToVacancies> urlsToVacancies = _uriToVacanciesService.GetAll(ChatId, siteType, CancelToken);
        if (urlsToVacancies.Count <= 0)
        {
            ShowSitesThenShowUrisToVacancies(next);
            return;
        }
        Push("Виберіть потрібне посилання.");
        Pager(urlsToVacancies, page, indexToUrl => (indexToUrl.WithoutHttps, Q(next, indexToUrl.Id, siteType)),
                                        Q(ShowUrisToVacancies, FirstPage, siteType, next), 1);
        RowButton(Back, Q(ShowSitesThenShowUrisToVacancies, next));
    }

    [Action]
    private async Task GetUriToVacanciesAsync(ObjectId urlId, SiteType siteType)
    {
        UriToVacancies uriToVacancies = _uriToVacanciesService.Get(urlId, CancelToken);
        ActivateRowButton(urlId, uriToVacancies.IsActivated);
        await Send(uriToVacancies.OriginalString, new() { DisableWebPagePreview = true });
    }

}
