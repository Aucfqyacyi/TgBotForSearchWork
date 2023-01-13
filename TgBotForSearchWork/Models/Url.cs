using MongoDB.Bson.Serialization.Attributes;

namespace TgBotForSearchWork.Models;

public class UrlToVacancies
{
    private const int _lastVacanciesIdsSize = 5;
    private ulong[] _lastVacanciesIds = new ulong[_lastVacanciesIdsSize];

    [BsonElement] public Uri Uri { get; set; }
    [BsonElement] public bool IsOff { get; set; }
    [BsonElement] public IList<ulong> LastVacanciesIds 
    { 
        get => _lastVacanciesIds;  
        set 
        {
            if(_lastVacanciesIds is null)
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

    [BsonIgnore] public string OriginalString { get => Uri.OriginalString; }
    [BsonIgnore] public string Host { get => Uri.Host; }

    public UrlToVacancies(string url, bool isOff = false) 
    {
        Uri = new(url);
        IsOff = isOff;
    }
}
