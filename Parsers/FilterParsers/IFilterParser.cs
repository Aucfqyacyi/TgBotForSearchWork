using Parsers.Constants;
using Parsers.FilterParsers.Parsers;
using Parsers.Models;

namespace Parsers.FilterParsers;

public interface IFilterParser
{
    public Task<List<Filter>> ParseAsync(string url, CancellationToken cancellationToken = default);
}
