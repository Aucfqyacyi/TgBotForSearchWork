using AngleSharp;
using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;

namespace Parsers.FilterParsers;

internal abstract class FilterParser : IFilterParser
{
    protected readonly HttpClient _httpClient;

    protected FilterParser(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected abstract string SearchGetParamName { get; }

    public virtual async ValueTask<List<Filter>> ParseAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        FilterCategory filterCategory = new("Пошук", SearchGetParamName);
        GetParameter getParameter = new(SearchGetParamName, string.Empty);
        List<Filter> filters = new()
        {
            new Filter("Пошуковий запит", filterCategory, getParameter, FilterType.Text)
        };
        using Stream response = await _httpClient.GetStreamIfSuccessAsync(uri, cancellationToken);
        using IBrowsingContext browsingContext = BrowsingContext.New();
        using IDocument document = await browsingContext.OpenAsync(req => req.Content(response), cancellationToken);
        CollectFilters(document, filters);
        return filters;
    }

    protected abstract void CollectFilters(IDocument document, List<Filter> filters);

}
