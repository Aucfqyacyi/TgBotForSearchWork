using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;

namespace TgBotForSearchWorkApi.Models.States;

public struct AddingSearchFilterToUrlState
{
    public ObjectId? UrlId { get; set; }
    public SiteType SiteType { get; set; }
    public string GetParametrName { get; set; }

    public AddingSearchFilterToUrlState(ObjectId? urlId, string getParametrName, SiteType siteType)
    {
        GetParametrName = getParametrName;
        UrlId = urlId;
        SiteType = siteType;
    }
}
