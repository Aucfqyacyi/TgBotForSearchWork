﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Parsers.Constants;
using Parsers.Models;
using System.Web;

namespace TgBotForSearchWorkApi.Models;

public partial class UriToVacancies
{
    [BsonIgnore] public string OriginalString { get => Uri.OriginalString; }
    [BsonIgnore] public string WithoutHttps { get => Uri.Host + Uri.PathAndQuery; }

    public UriToVacancies(long userId, Uri uri)
    {
        Id = ObjectId.GenerateNewId();
        UserIds.Add(userId);
        Uri = uri;
        SiteType = SiteTypesToUris.HostsToSiteTypes[Uri.Host];
    }

    public UriToVacancies(long userId, string url) : this(userId, new Uri(url))
    {
    }


    public static implicit operator string(UriToVacancies url)
    {
        return url.OriginalString;
    }

    public void AddGetParametr(string getParametrName, string getParametrValue)
    {
        UriBuilder uriBuilder = new(Uri);
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        if (query.GetValues(getParametrName) is null)
        {
            query.Add(getParametrName, getParametrValue);
            uriBuilder.Query = query.ToString();
            Uri = uriBuilder.Uri;
        }        
    }
}