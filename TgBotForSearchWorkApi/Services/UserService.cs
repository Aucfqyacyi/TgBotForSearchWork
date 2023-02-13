using Deployf.Botf;
using MongoDB.Bson;
using Parsers.Constants;
using Parsers.Models;
using Parsers.VacancyParsers;
using System.Web;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Repositories;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class UserService
{
    private readonly UserRepository _userRepository;
    private readonly UrlToVacanciesRepository _urlToVacanciesRepository;

    public UserService(UserRepository userRepository, UrlToVacanciesRepository urlToVacanciesRepository)
    {
        _userRepository = userRepository;
        _urlToVacanciesRepository = urlToVacanciesRepository;
    }

    public async Task<UrlToVacancies?> AddUrlToVacancyAsync(long userId, string url, CancellationToken cancellationToken)
    {
        try
        {
            if (url.IsUrl() is false)
                return null;
            Uri uri = new Uri(url);
            IVacancyParser vacancyParser = VacancyParserFactory.CreateVacancyParser(uri);
            if (await vacancyParser.IsCorrectUrlAsync(uri, cancellationToken) is false)
                return null;
            UrlToVacancies urlToVacancies = new(userId, uri);
            _userRepository.AddUrlToVacancies(userId, urlToVacancies.Id, cancellationToken);
            return _urlToVacanciesRepository.InsertOrUpdate(userId, urlToVacancies, cancellationToken);
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
        return null;
    }

    public void RemoveUrlToVacancyAsync(long userId, ObjectId urlId, CancellationToken cancellationToken)
    {
        _userRepository.RemoveUrlToVacancies(userId, urlId, cancellationToken);
    }

    public bool AddUrlToVacancyAsync(long userId, ObjectId urlId, CancellationToken cancellationToken)
    {
        return _userRepository.AddUrlToVacancies(userId, urlId, cancellationToken);
    }

}
