﻿using AngleSharp;
using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Models;

namespace Parsers.FilterParsers;

internal abstract class FilterParser : IFilterParser
{
    protected abstract string SearchGetParamName { get; }

    public async Task<List<Filter>> ParseAsync(Uri uri, CancellationToken cancellationToken = default)
    {
        List<Filter> filters = new()
        {
            new("Пошук", new("Пошук"), new(SearchGetParamName, string.Empty), FilterType.Text)
        };
        using Stream response = await GlobalHttpClient.GetAsync(uri, cancellationToken);
        using IBrowsingContext browsingContext = BrowsingContext.New();
        using IDocument document = await browsingContext.OpenAsync(req => req.Content(response), cancellationToken);
        CollectFilters(document, filters);
        return filters;
    }

    protected abstract void CollectFilters(IDocument document, List<Filter> filters);

}