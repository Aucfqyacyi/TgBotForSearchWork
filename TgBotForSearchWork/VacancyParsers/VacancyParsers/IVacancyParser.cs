using TgBotForSearchWork.VacancyParsers.Models;

namespace TgBotForSearchWork.VacancyParsers.VacancyParsers;

public interface IAllVacancyParser
{
    public Task<List<Vacancy>> ParseAsync(Stream stream, string host, CancellationToken cancellationToken = default);
}
