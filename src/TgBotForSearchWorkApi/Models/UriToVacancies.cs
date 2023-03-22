using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Parsers.Constants;
using TgBotForSearchWorkApi.Extensions;

namespace TgBotForSearchWorkApi.Models;

public partial class UriToVacancies
{
    private const int _lastVacanciesIdsSize = 5;
    private List<ulong> _lastVacanciesIds = new(_lastVacanciesIdsSize);

    [BsonId] public ObjectId Id { get; set; }
    [BsonElement] public long ChatId { get; set; }
    [BsonElement] public Uri Uri { get; set; }
    [BsonElement] public string HashedUrl { get => Uri.OriginalString.GetMD5(); }
    [BsonElement] public bool IsActivated { get; set; }
    [BsonElement] public SiteType SiteType { get; set; }
    [BsonElement] public IReadOnlyList<ulong> LastVacanciesIds
    {
        get => _lastVacanciesIds;
        set
        {
            if (value.Count >= _lastVacanciesIdsSize || _lastVacanciesIds is null)
            {
                int count = int.Min(value.Count, _lastVacanciesIdsSize);
                _lastVacanciesIds = new List<ulong>(value.Take(count));
            }
            else
            {
                List<ulong> newLastVacanciesIds = new(value);
                if(_lastVacanciesIds.Any() is true)
                    newLastVacanciesIds.AddRange(_lastVacanciesIds.GetRange(0, _lastVacanciesIdsSize - value.Count));
                _lastVacanciesIds = newLastVacanciesIds;
            }
        }
    }


}
