using Parsers.Constants;
using Parsers.FilterParsers.Parsers;
using Parsers.Models;

namespace Parsers.FilterParsers;

public interface IFilterParser
{
    public Task<List<Filter>> ParseAsync(CancellationToken cancellationToken = default);

    public static async Task Test()
    {
        WorkUaFilterParser a = new();
        await a.ParseAsync();
    }
}
