using FilterParserLibrary.Models;

namespace FilterParserLibrary.FilterParsers;

public interface IFilterParser
{
    public Task<List<Filter>> ParseAsync(Uri uri, CancellationToken cancellationToken = default);
}
