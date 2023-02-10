using Parsers.Models;

namespace Parsers.VacancyParsers;

public interface IVacancyParser
{
    public Task<List<Vacancy>> ParseAsync(Uri uri, CancellationToken cancellationToken = default);
    public Task<bool> IsCorrectUrlAsync(Uri uri, CancellationToken cancellationToken = default);
}
