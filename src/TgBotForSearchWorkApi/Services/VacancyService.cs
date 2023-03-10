using AutoDIInjector.Attributes;
using Parsers.Models;
using Parsers.VacancyParsers;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class VacancyService
{
    public async ValueTask<List<Vacancy>> GetRelevantVacanciesAsync(IEnumerable<UriToVacancies> urisToVacancies, int descriptionLength,
                                                                    CancellationToken cancellationToken)
    {
        List<Vacancy> vacancies = new();
        await Parallel.ForEachAsync(urisToVacancies, cancellationToken,
                            (UriToVacancies uriToVacancies, CancellationToken cancellationToken) =>
        {
            return GetRelevantVacanciesAsync(uriToVacancies, vacancies, descriptionLength, cancellationToken);
        });
        return vacancies;
    }

    private async ValueTask GetRelevantVacanciesAsync(UriToVacancies uriToVacancies, List<Vacancy> vacancies, int descriptionLength,
                                                      CancellationToken cancellationToken)
    {
        try
        {
            IVacancyParser vacancyParser = VacancyParserFactory.Create(uriToVacancies.Uri);
            List<Vacancy> relevantVacancies = await vacancyParser.ParseAsync(uriToVacancies.Uri, descriptionLength, 
                                                                             uriToVacancies.LastVacanciesIds, cancellationToken);

            Log.Info($"{uriToVacancies.Uri.Host} has number of vacancies {relevantVacancies.Count}");
            if (relevantVacancies.Count == 0)
                return;
            uriToVacancies.LastVacanciesIds = relevantVacancies.Select(vacancy => vacancy.Id).ToList();
            lock (vacancies)
                vacancies.AddRange(relevantVacancies);
        }
        catch (Exception ex)
        {
            Log.Info(ex.Message);
        }      
    }
}
