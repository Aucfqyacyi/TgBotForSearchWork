using Parsers.Models;

namespace Parsers.VacancyParsers;

public interface IVacancyParser
{
    public Task<List<Vacancy>> ParseAsync(Uri uri, CancellationToken cancellationToken = default);
}
