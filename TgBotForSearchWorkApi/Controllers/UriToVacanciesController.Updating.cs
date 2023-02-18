using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;

namespace TgBotForSearchWorkApi.Controllers;


public partial class UriToVacanciesController
{
    [Action(Command.UpdateUrl, CommandDescription.Empty)]
    public void UpdateUrl()
    {
        GetSiteNamesThenGetUrlsToVacancies(UpdateUriToVacancies);
    }

    [Action]
    private void UpdateUriToVacancies(ObjectId urlId, SiteType siteType)
    {
        GetFilterCategories(0, urlId, siteType, _uriToVacanciesService.IsActivated(urlId, CancelToken));
    }
}
