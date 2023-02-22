using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;
using TgBotForSearchWorkApi.Models;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{

    [Action(Command.GetUrl, CommandDescription.Empty)]
    public void GetUrl()
    {
        ShowSiteNamesThenShowUrlsToVacancies(ShowUriToVacancies);
    }

    [Action]
    protected void ShowSiteNamesThenShowUrlsToVacancies(Delegate next)
    {
        ShowSiteNames(siteType => Q(GetUrlsToVacanciesAsync, 0, siteType, next));
    }

    [Action]
    protected async Task GetUrlsToVacanciesAsync(int page, SiteType siteType, Delegate next)
    {

        List<UriToVacancies> urlsToVacancies = _uriToVacanciesService.GetAll(ChatId, siteType, CancelToken);
        if (urlsToVacancies.Count <= 0)
        {
            await Send("У вас немає посилань.");
            return;
        }
        Push("Виберіть, потрібне посилання.");
        Pager(urlsToVacancies, page, indexToUrl => (indexToUrl.WithoutHttps, Q(next, indexToUrl.Id, siteType)),
                                        Q(GetUrlsToVacanciesAsync, FirstPage, siteType, next), 1);
        RowButton(Back, Q(ShowSiteNamesThenShowUrlsToVacancies, next));
    }

    [Action]
    private async Task ShowUriToVacancies(ObjectId urlId, SiteType siteType)
    {
        await AnswerCallback();
        UriToVacancies uriToVacancies = _uriToVacanciesService.Get(urlId, CancelToken);
        ActivateRowButton(uriToVacancies.Id, uriToVacancies.IsActivated);
        await Send(uriToVacancies.OriginalString, new() { DisableWebPagePreview = true });
    }

}
