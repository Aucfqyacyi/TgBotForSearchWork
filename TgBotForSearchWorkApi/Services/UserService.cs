using Deployf.Botf;
using MongoDB.Bson;
using Parsers.VacancyParsers;
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
            UrlToVacancies urlToVacancies = _urlToVacanciesRepository.InsertOrUpdate(userId, new(userId, uri), cancellationToken);
            bool result = _userRepository.AddUrlToVacancies(userId, urlToVacancies.Id, cancellationToken);
            if (result)
                return urlToVacancies;
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
        return null;
    }

    public void RemoveUrlToVacancy(long userId, ObjectId urlId, CancellationToken cancellationToken)
    {
        _userRepository.RemoveUrlToVacancies(userId, urlId, cancellationToken);
    }

}
