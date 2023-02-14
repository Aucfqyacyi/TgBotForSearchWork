using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using TgBotForSearchWorkApi.Constants;

namespace TgBotForSearchWorkApi.Controllers;

public partial class UrlToVacanciesController
{
    [Action(Command.RemoveUrl, CommandDescription.Empty)]
    public void RemoveUrl()
    {
        GetSiteNamesThenGetUrlsToVacancies(RemoveUrlToVacancies);
    }

    [Action]
    private async Task RemoveUrlToVacancies(ObjectId urlId, SiteType siteType)
    {
        _userService.RemoveUrlToVacancy(ChatId, urlId, CancelToken);
        _urlToVacanciesService.RemoveUserId(urlId, ChatId, CancelToken);
        await AnswerOkCallback();
        await GetUrlsToVacanciesAsync(0, siteType, RemoveUrlToVacancies);
    }
}
