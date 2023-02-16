using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Parsers.Constants;
using TgBotForSearchWorkApi.Extensions;

namespace TgBotForSearchWorkApi.Models;

public partial class UriToVacancies
{
    private const int _lastVacanciesIdsSize = 5;
    private ulong[] _lastVacanciesIds = new ulong[_lastVacanciesIdsSize];

    [BsonId] public ObjectId Id { get; set; }
    [BsonElement] public long ChatId { get; set; }
    [BsonElement] public Uri Uri { get; set; }
    [BsonElement] public string HashedUrl { get => Uri.OriginalString.GetMD5(); }
    [BsonElement] public bool IsActivate {get; set;} = false;
    [BsonElement] public SiteType SiteType { get; set; }

    [BsonElement] public IList<ulong> LastVacanciesIds
    {
        get => _lastVacanciesIds;
        set
        {
            if (_lastVacanciesIds is null)
                _lastVacanciesIds = new ulong[_lastVacanciesIdsSize];

            int i = 0;
            for (; i < Math.Min(value.Count, _lastVacanciesIdsSize); i++)
            {
                _lastVacanciesIds[i] = value[i];
            }

            for (; i < _lastVacanciesIdsSize; i++)
                _lastVacanciesIds[i] = 0;

        }
    }


}
