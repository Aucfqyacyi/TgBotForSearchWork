using AutoDIInjector.Attributes;
using Parsers.Models;
using Parsers.ParserFactories;
using Parsers.VacancyParsers;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class VacancyService
{
    private readonly VacancyParserFactory _vacancyParserFactory;
    private readonly SemaphoreSlim _semaphoreSlim = new(1);

    public VacancyService(VacancyParserFactory vacancyParserFactory)
    {
        _vacancyParserFactory = vacancyParserFactory;
    }

    public async ValueTask<List<Vacancy>> GetRelevantVacanciesAsync(IEnumerable<UriToVacancies> urisToVacancies, uint descriptionLength,
                                                                    CancellationToken cancellationToken)
    {
        List<Vacancy> vacancies = new();
        await Parallel.ForEachAsync(urisToVacancies, cancellationToken, (uriToVacancies, cancellationToken) =>
        {
            return GetRelevantVacanciesAsync(uriToVacancies, vacancies, descriptionLength, cancellationToken);
        });
        return vacancies;
    }

    private async ValueTask GetRelevantVacanciesAsync(UriToVacancies uriToVacancies, List<Vacancy> vacancies, uint descriptionLength,
                                                      CancellationToken cancellationToken)
    {
        try
        {
            IVacancyParser vacancyParser = _vacancyParserFactory.GetOrCreate(uriToVacancies.Uri);
            List<Vacancy> relevantVacancies = await vacancyParser.ParseAsync(uriToVacancies.Uri, descriptionLength,
                                                                             uriToVacancies.LastVacanciesIds, cancellationToken);

            Log.Info($"{uriToVacancies.Uri.Host} has number of vacancies {relevantVacancies.Count}");
            if (relevantVacancies.Count == 0)
                return;
            uriToVacancies.LastVacanciesIds = relevantVacancies.Select(vacancy => vacancy.Id).ToList();
            await _semaphoreSlim.WaitAsync(cancellationToken);
            vacancies.AddRange(relevantVacancies);
            _semaphoreSlim.Release();
        }
        catch (Exception ex)
        {
            Log.Info($"Inside VacancyService.GetRelevantVacanciesAsync was thrown exception, error - {ex.Message}");
        }
    }
}
