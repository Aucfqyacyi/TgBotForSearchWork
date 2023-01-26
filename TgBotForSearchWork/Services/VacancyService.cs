using Parsers.Models;
using Parsers.VacancyParsers;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWork.Services;


internal class VacancyService
{
    public async Task<List<Vacancy>> GetRelevantVacanciesAsync(User user, CancellationToken cancellationToken)
    {
        List<Vacancy> vacancies = new();
        await Parallel.ForEachAsync(user.Urls, cancellationToken, (UrlToVacancies urlToVacancies, CancellationToken cancellationToken) =>
        {
            return GetRelevantVacanciesAsync(urlToVacancies, vacancies, cancellationToken);
        });
        return vacancies;
    }

    private async ValueTask GetRelevantVacanciesAsync(UrlToVacancies urlToVacancies, List<Vacancy> vacancies, CancellationToken cancellationToken)
    {
        List<Vacancy> relevantVacancies = await GetRelevantVacanciesAsync(urlToVacancies, cancellationToken);
        Log.Info($"{urlToVacancies.Host} has number of vacancies {relevantVacancies.Count}");
        if (relevantVacancies.Count == 0)
            return;
        urlToVacancies.LastVacanciesIds = relevantVacancies.Select(e => e.Id).ToList();        
        lock (this)
            vacancies.AddRange(relevantVacancies);
    }

    private async Task<List<Vacancy>> GetRelevantVacanciesAsync(UrlToVacancies urlToVacancies, CancellationToken cancellationToken)
    {
        IVacancyParser vacancyParser = VacancyParserFactory.CreateVacancyParser(urlToVacancies.Uri);
        List<Vacancy> vacancies = await vacancyParser.ParseAsync(urlToVacancies.Uri, cancellationToken);
        for (int i = 0; i < Math.Min(vacancies.Count, urlToVacancies.LastVacanciesIds.Count); i++)
        {
            if (urlToVacancies.LastVacanciesIds[i] == 0)
                break;
            if (vacancies[i].Id == urlToVacancies.LastVacanciesIds[i])
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
