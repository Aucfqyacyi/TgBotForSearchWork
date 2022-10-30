using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.VacancyParsers;

public interface IVacancyParser
{
    public Task<List<Vacancy>> ParseAsync(Uri uri, CancellationToken cancellationToken = default);
}
