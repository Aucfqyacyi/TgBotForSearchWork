using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Parsers.Constants;
using Parsers.Models;
using System.Collections.Specialized;
using System.Web;

namespace TgBotForSearchWorkApi.Models;

public partial class UriToVacancies
{
    [BsonIgnore] public string OriginalString { get => Uri.OriginalString; }
    [BsonIgnore] public string WithoutHttps { get => Uri.Host + Uri.PathAndQuery; }

    public UriToVacancies(long chatId, Uri uri, bool isActivated = false)
    {
        Id = ObjectId.GenerateNewId();
        ChatId = chatId;
        Uri = uri;
        IsActivated = isActivated;
        SiteType = SiteTypesToUris.HostsToSiteTypes[Uri.Host];
    }

    public UriToVacancies(long chatId, string url, bool isActivated = false) : this(chatId, new Uri(url), isActivated)
    {
    }


    public static implicit operator string(UriToVacancies url)
    {
        return url.OriginalString;
    }

    public void AddGetParameter(GetParameter getParametr)
    {
        UriBuilder uriBuilder = new(Uri);
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        if (query.GetValues(getParametr.Name) is null)
            query.Add(getParametr.Name, getParametr.Value);
        else
            query[getParametr.Name] = getParametr.Value;
        uriBuilder.Query = query.ToString();
        Uri = uriBuilder.Uri;
    }

    public void RemoveGetParameter(GetParameter getParametr)
    {
        UriBuilder uriBuilder = new(Uri);
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        if (query.GetValues(getParametr.Name) is not null)
        {
            query.Remove(getParametr.Name);
            uriBuilder.Query = query.ToString();
            Uri = uriBuilder.Uri;
        }
    }

    public List<GetParameter> GetParameters()
    {
        UriBuilder uriBuilder = new(Uri);
        List<GetParameter> getParametrs = new List<GetParameter>();
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        for (int i = 0; i < query.Count; i++)
        {
            getParametrs.Add(new(query.GetKey(i)!, query.Get(i)!));
        }
        return getParametrs;
    }
}
