using Parsers.Models;

namespace Parsers.VacancyParsers;

public interface IVacancyParser
{
    public ValueTask<List<Vacancy>> ParseAsync(Uri uri, int descriptionLength, CancellationToken cancellationToken = default);
    public ValueTask<bool> IsCorrectUrlAsync(Uri uri, CancellationToken cancellationToken = default);
}
