using MongoDB.Bson;
using Parsers.Constants;

namespace TgBotForSearchWorkApi.Models.States;

public struct AddingSearchFilterToUrlState
{
    public ObjectId? UrlId { get; set; }
    public SiteType SiteType { get; set; }
    public string GetParameterName { get; set; }

    public AddingSearchFilterToUrlState(ObjectId? urlId, string getParameterName, SiteType siteType)
    {
        GetParameterName = getParameterName;
        UrlId = urlId;
        SiteType = siteType;
    }
}
