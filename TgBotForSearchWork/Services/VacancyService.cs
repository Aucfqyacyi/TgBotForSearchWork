using TgBotForSearchWork.Models;
using TgBotForSearchWork.Utilities;
using TgBotForSearchWork.VacancyParsers;

namespace TgBotForSearchWork.Services;


internal class VacancyService
{

    public async Task<List<Vacancy>> GetRelevantVacancies(User user, CancellationToken cancellationToken = default)
    {
        List<Vacancy> allVacancies = new();
        foreach (var urlToVacancies in user.Urls)
        {
            if (!urlToVacancies.IsOff)
            {
                List<Vacancy> relevantVacancies = await GetRelevantVacancies(urlToVacancies, cancellationToken);
                if (relevantVacancies.Count != 0)
                {                   
                    urlToVacancies.LastVacanciesIds = relevantVacancies.Select(e => e.Id).ToList();
                }
                Log.Info($"{urlToVacancies.Host} has number of vacancies {relevantVacancies.Count}");
                allVacancies.AddRange(relevantVacancies);
            }
        }
        return allVacancies;
    }

    private async Task<List<Vacancy>> GetRelevantVacancies(UrlToVacancies urlToVacancies, CancellationToken cancellationToken = default)
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
