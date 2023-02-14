using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UriToVacanciesController
{
    [Action(Command.RemoveUrl, CommandDescription.Empty)]
    public void RemoveUrl()
    {
        GetSiteNamesThenGetUrlsToVacancies(RemoveUriToVacancies);
    }

    [Action]
    private async Task RemoveUriToVacancies(ObjectId urlId, SiteType siteType)
    {
        _userService.RemoveUrlToVacancy(ChatId, urlId, CancelToken);
        _uriToVacanciesService.RemoveUserId(urlId, ChatId, CancelToken);
        await AnswerOkCallback();
        await GetUrlsToVacanciesAsync(0, siteType, RemoveUriToVacancies);
    }
}
