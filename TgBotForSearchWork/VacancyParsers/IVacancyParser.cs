using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.VacancyParsers;

public interface IAllVacancyParser
{
    public Task<List<Vacancy>> ParseAsync(Stream stream, string host, CancellationToken cancellationToken = default);
}
