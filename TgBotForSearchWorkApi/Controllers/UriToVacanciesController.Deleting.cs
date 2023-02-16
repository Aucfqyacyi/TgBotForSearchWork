using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    [Action(Command.DeleteUrl, CommandDescription.Empty)]
    public void DeleteUrl()
    {
        GetSiteNamesThenGetUrlsToVacancies(DeleteUriToVacancies);
    }

    [Action]
    private async Task DeleteUriToVacancies(ObjectId urlId, SiteType siteType)
    {
        _uriToVacanciesService.Delete(urlId, CancelToken);
        await AnswerOkCallback();
        await GetUrlsToVacanciesAsync(0, siteType, DeleteUriToVacancies);
    }
}
