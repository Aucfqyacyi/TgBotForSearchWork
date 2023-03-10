using Parsers.Models;

namespace Parsers.VacancyParsers;

public interface IVacancyParser
{
    public ValueTask<List<Vacancy>> ParseAsync(Uri uri, int descriptionLength, IReadOnlyList<ulong>? vacancyIdsToIgnore = null, CancellationToken cancellationToken = default);
    public ValueTask<bool> IsCorrectUriAsync(Uri uri, CancellationToken cancellationToken = default);
}
