using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.FilterParsers;

public interface IFilterParser
{
    public IReadOnlyList<Filter> Filters { get; }
    public Task ParseAsync(Uri uri, CancellationToken cancellationToken = default);
}
