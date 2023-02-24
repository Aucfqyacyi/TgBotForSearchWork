using Parsers.Models;
using Parsers.VacancyParsers;
using TgBotForSearchWorkApi.Models;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWorkApi.Services;

[SingletonService]
public class VacancyService
{
    public async ValueTask<List<Vacancy>> GetRelevantVacanciesAsync(IEnumerable<UriToVacancies> urisToVacancies, CancellationToken cancellationToken)
    {
        List<Vacancy> vacancies = new();
        await Parallel.ForEachAsync(urisToVacancies, cancellationToken, 
                            (UriToVacancies uriToVacancies, CancellationToken cancellationToken) =>
        {
            return GetRelevantVacanciesAsync(uriToVacancies, vacancies, cancellationToken);
        });
        return vacancies;
    }

    private async ValueTask GetRelevantVacanciesAsync(UriToVacancies uriToVacancies, List<Vacancy> vacancies, CancellationToken cancellationToken)
    {
        List<Vacancy> relevantVacancies = await GetRelevantVacanciesAsync(uriToVacancies, cancellationToken);
        Log.Info($"{uriToVacancies.Uri.Host} has number of vacancies {relevantVacancies.Count}");
        if (relevantVacancies.Count == 0)
            return;
        uriToVacancies.LastVacanciesIds = relevantVacancies.Select(e => e.Id).ToList();        
        lock (this)
            vacancies.AddRange(relevantVacancies);
    }

    private async ValueTask<List<Vacancy>> GetRelevantVacanciesAsync(UriToVacancies uriToVacancies, CancellationToken cancellationToken)
    {
        IVacancyParser vacancyParser = VacancyParserFactory.Create(uriToVacancies.Uri);
        List<Vacancy> vacancies = await vacancyParser.ParseAsync(uriToVacancies.Uri, 0, cancellationToken);
        for (int i = 0; i < Math.Min(vacancies.Count, uriToVacancies.LastVacanciesIds.Count); i++)
        {
            if (uriToVacancies.LastVacanciesIds[i] == 0)
                break;
            if (vacancies[i].Id == uriToVacancies.LastVacanciesIds[i])
            {
                if (i == 0)
                    return new();
                else
                    return vacancies.GetRange(0, i);
            }
        }
        return vacancies;
    }


}
