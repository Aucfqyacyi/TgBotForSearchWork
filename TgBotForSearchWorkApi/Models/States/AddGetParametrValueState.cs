using Parsers.Constants;
using Parsers.Models;

namespace TgBotForSearchWorkApi.Models.States;

public struct AddGetParametrValueState
{
    public int UrlIndex { get; set; }
    public SiteType SiteType { get; set; }
    public Filter Filter { get; set; }

    public AddGetParametrValueState(int urlIndex, Filter filter, SiteType siteType)
    {
        Filter = filter;
        UrlIndex = urlIndex;
        SiteType = siteType;
    }
}
