using Parsers.Models;
using System.Diagnostics.CodeAnalysis;

namespace Parsers.IntegrationTests.Utilities;

internal class VacancyComparer : IEqualityComparer<Vacancy>
{
    public bool Equals(Vacancy? x, Vacancy? y)
    {
        return x?.Id == y?.Id;
    }

    public int GetHashCode([DisallowNull] Vacancy obj)
    {
        throw new NotImplementedException();
    }
}
