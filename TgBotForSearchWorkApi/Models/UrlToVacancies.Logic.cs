using MongoDB.Bson.Serialization.Attributes;

namespace TgBotForSearchWorkApi.Models;

public partial class UrlToVacancies
{
    [BsonIgnore] public string OriginalString { get => Uri.OriginalString; }
    [BsonIgnore] public string WithOutHttps { get => Uri.Host + Uri.PathAndQuery; }
    [BsonIgnore] public string Host { get => Uri.Host; }

    public UrlToVacancies(Uri uri)
    {
        Uri = uri;
    }

    public UrlToVacancies(string url) : this(new Uri(url))
    {
    }

}
