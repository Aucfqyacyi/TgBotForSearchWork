using TgBotForSearchWork.VacancyParsers.Models;

namespace TgBotForSearchWork.VacancyParsers.AllVacancyParsers;

public interface IAllVacancyParser
{
    public Task<List<Vacancy>> ParseAsync(Stream stream, string host, CancellationToken cancellationToken = default);
}
