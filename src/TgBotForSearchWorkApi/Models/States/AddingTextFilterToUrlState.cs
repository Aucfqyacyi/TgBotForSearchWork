using MongoDB.Bson;
using Parsers.Constants;

namespace TgBotForSearchWorkApi.Models.States;

public struct AddingTextFilterToUrlState
{
    public ObjectId? UrlId { get; set; }
    public SiteType SiteType { get; set; }
    public string GetParameterName { get; set; }
    public bool IsUpdating { get; set; }

    public AddingTextFilterToUrlState(ObjectId? urlId, string getParameterName, SiteType siteType, bool isUpdating)
    {
        GetParameterName = getParameterName;
        UrlId = urlId;
        SiteType = siteType;
        IsUpdating = isUpdating;
    }
}
