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


    public UriToVacancies(long chatId, Uri uri, SiteType siteType, bool isActivated = false)
    {
        Id = ObjectId.GenerateNewId();
        ChatId = chatId;
        Uri = uri;
        IsActivated = isActivated;
        SiteType = siteType;
    }

    public UriToVacancies(long chatId, Uri uri, bool isActivated = false)
                                        : this(chatId, uri, SiteTypesToUris.HostsToSiteTypes[uri.Host], isActivated)
    { }

    public static implicit operator string(UriToVacancies url)
    {
        return url.OriginalString;
    }

    public void AddGetParameter(GetParameter getParametr)
    {
        UriBuilder uriBuilder = new(Uri);
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        if (query.HasKeys() is true)
        {
            if (getParametr.CanBeDuplicated is true)
                query.Add(getParametr.Name, getParametr.Value);
            else
                query[getParametr.Name] = getParametr.Value;
        }
        else
        {
            query.Add(getParametr.Name, getParametr.Value);
        }
        UpdateUri(uriBuilder, query.ToString());
    }

    public void RemoveGetParameter(GetParameter getParametr)
    {
        UriBuilder uriBuilder = new(Uri);
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        if (query.HasKeys() is true)
        {
            query.Remove(getParametr.Name);
            UpdateUri(uriBuilder, query.ToString());
        }
    }

    public static void AddGetParameter(UriToVacancies uriToVacancies, GetParameter getParametr)
    {
        uriToVacancies.AddGetParameter(getParametr);
    }

    public static void RemoveGetParameter(UriToVacancies uriToVacancies, GetParameter getParametr)
    {
        uriToVacancies.RemoveGetParameter(getParametr);
    }

    private void UpdateUri(UriBuilder uriBuilder, string? newQuery)
    {
        uriBuilder.Query = newQuery;
        Uri = uriBuilder.Uri;
    }

    public List<GetParameter> GetParameters()
    {
        UriBuilder uriBuilder = new(Uri);
        List<GetParameter> getParametrs = new();
        NameValueCollection query = HttpUtility.ParseQueryString(uriBuilder.Query);
        for (int i = 0; i < query.Count; i++)
        {
            getParametrs.Add(new(query.GetKey(i)!, query.Get(i)!));
        }
        return getParametrs;
    }
}
