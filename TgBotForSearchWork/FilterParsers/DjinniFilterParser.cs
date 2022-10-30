using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.FilterParsers;
internal class DjinniFilterParser : IFilterParser
{
    public IReadOnlyList<Filter> Filters => throw new NotImplementedException();

    public Task ParseAsync(Uri uri, CancellationToken cancellationToken = default)
    {
       return Task.CompletedTask;
    }
}
