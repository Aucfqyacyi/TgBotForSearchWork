using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Models;
using Parsers.Utilities;

namespace Parsers.FilterParsers;

internal abstract class FilterParser : IFilterParser
{
    protected abstract string SearchGetParamName { get; }
    protected abstract string UriToMainPage { get; }

    public async Task<List<Filter>> ParseAsync(CancellationToken cancellationToken = default)
    {
        List<Filter> filters = new();
        filters.Add(new("Пошук", string.Empty, SearchGetParamName, Enums.FilterType.Text));
        using Stream response = await GlobalHttpClient.GetAsync(UriToMainPage, cancellationToken);
        using IDocument doc = await HtmlDocument.CreateAsync(response, cancellationToken);
        CollectFilters(doc, filters);
        return filters;
    }

    protected abstract void CollectFilters(IDocument doc, List<Filter> filters);

}
