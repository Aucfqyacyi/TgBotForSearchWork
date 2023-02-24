using Parsers.Constants;
using Parsers.FilterParsers.Parsers;
using Parsers.Models;

namespace Parsers.FilterParsers;

public interface IFilterParser
{
    public ValueTask<List<Filter>> ParseAsync(Uri url, CancellationToken cancellationToken = default);
}
