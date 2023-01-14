using Parsers.Models;

namespace Parsers.FilterParsers;

public interface IFilterParser
{
    public Task<List<Filter>> ParseAsync(Uri uri, CancellationToken cancellationToken = default);
}
