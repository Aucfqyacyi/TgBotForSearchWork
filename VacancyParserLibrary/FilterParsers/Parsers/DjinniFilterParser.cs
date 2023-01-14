using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;
using Parsers.Utilities;

namespace Parsers.FilterParsers.Parsers;


internal class DjinniFilterParser : IFilterParser
{
    protected readonly Uri _uriToMainPage = new(Host.Https + Host.All[SiteType.Djinni] + @"/jobs/");
    protected HtmlElement Set { get; } = new("jobs-filter__set");
    protected HtmlElement FilterLink { get; } = new("jobs-filter__link", "a");


    public async Task<List<Filter>> ParseAsync(CancellationToken cancellationToken = default)
    {
        List<Filter> filters = new();
        filters.Add(new("Пошук", string.Empty, "keywords", Enums.FilterType.Text));
        using Stream response = await GlobalHttpClient.GetAsync(_uriToMainPage, cancellationToken);
        using IDocument doc = await HtmlDocument.CreateAsync(response, cancellationToken);
        IHtmlCollection<IElement> sets = doc.GetElementsByClassName(Set.CssClassName);
        foreach (var set in sets)
        {
            CollectFiltersFromSet(set, filters);
        }
        return filters;
    }

    protected void CollectFiltersFromSet(IElement set, List<Filter> filters)
    {
        string category = set.PreviousElementSibling?.GetFirstChildTextContent() ?? string.Empty;
        IHtmlCollection<IElement>? filterElements = set.GetIElements(FilterLink);
        if (filterElements != null)
        {
            foreach (var filterElement in filterElements)
            {
                filters.Add(CreateFilter(filterElement, category));
            }
        }
    }

    protected Filter CreateFilter(IElement filterElement, string category)
    {
        string filterName = filterElement.GetFirstChildTextContent();
        string filterGetParamater = filterElement.GetHrefAttribute().TrimStart('?');
        return new(filterName, category, filterGetParamater, Enums.FilterType.CheckBox);
    }
}
