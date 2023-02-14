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
    private readonly UriToVacanciesRepository _uriToVacanciesRepository;

    public UserService(UserRepository userRepository, UriToVacanciesRepository uriToVacanciesRepository)
    {
        _userRepository = userRepository;
        _uriToVacanciesRepository = uriToVacanciesRepository;
    }

    public async Task<UriToVacancies?> AddUrlToVacancyAsync(long userId, string url, CancellationToken cancellationToken)
    {
        try
        {
            if (url.IsUrl() is false)
                return null;
            Uri uri = new Uri(url);
            IVacancyParser vacancyParser = VacancyParserFactory.CreateVacancyParser(uri);
            if (await vacancyParser.IsCorrectUrlAsync(uri, cancellationToken) is false)
                return null;
            UriToVacancies uriToVacancies = _uriToVacanciesRepository.InsertOrUpdate(userId, new(userId, uri), cancellationToken);
            bool result = _userRepository.AddUriToVacancies(userId, uriToVacancies.Id, cancellationToken);
            if (result)
                return uriToVacancies;
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }
        return null;
    }

    public void RemoveUrlToVacancy(long userId, ObjectId urlId, CancellationToken cancellationToken)
    {
        _userRepository.RemoveUriToVacancies(userId, urlId, cancellationToken);
    }

}
