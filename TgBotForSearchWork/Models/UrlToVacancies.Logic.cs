using MongoDB.Bson.Serialization.Attributes;
using Parsers.Constants;

namespace TgBotForSearchWork.Models;


public partial class UrlToVacancies
{

    [BsonIgnore] public string OriginalString { get => Uri.OriginalString; }
    [BsonIgnore] public string WithOutHttps { get => Uri.Host + Uri.PathAndQuery; }
    [BsonIgnore] public string Host { get => Uri.Host; }

    public UrlToVacancies(string url, bool isOff = false)
    {
        Uri = new(url);
        IsOff = isOff;
    }
}
