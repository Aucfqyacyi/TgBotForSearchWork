using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    [Action(Command.DeleteUrl, CommandDescription.DeleteUrl)]
    public void DeleteUrl()
    {
        ShowSitesThenShowUrisToVacancies(DeleteUriToVacancies);
    }

    [Action]
    private async Task DeleteUriToVacancies(ObjectId urlId, SiteType siteType)
    {
        await _uriToVacanciesService.DeleteAsync(urlId, CancelToken);
        await AnswerOkCallback();
        await ShowUrisToVacancies(0, siteType, DeleteUriToVacancies);
    }
}
