using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;

namespace Parsers.FilterParsers.Parsers;


internal class DjinniFilterParser : FilterParser
{
    protected override string SearchGetParamName { get; } = "keywords";

    protected HtmlElement _filterSet = new("jobs-filter__set");
    protected HtmlElement _filterLink = new("jobs-filter__link", "a");

    public DjinniFilterParser(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override void CollectFilters(IDocument document, List<Filter> filters)
    {
        IHtmlCollection<IElement> sets = document.GetElementsByClassName(_filterSet.CssClassName);
        foreach (var set in sets)
            CollectFiltersFromSet(set, filters);
    }

    protected void CollectFiltersFromSet(IElement set, List<Filter> filters)
    {
        List<IElement>? filterElements = set.GetElements(_filterLink);
        if (filterElements == null)
            return;
        string? categoryName = set.PreviousElementSibling?.GetTextContent();
        if (categoryName.IsNullOrEmpty())
            return;
        FilterCategory category = new(categoryName!);
        foreach (var filterElement in filterElements)
            filters.Add(CreateFilter(category, filterElement));
    }

    protected Filter CreateFilter(FilterCategory category, IElement filterElement)
    {
        string filterName = filterElement.GetTextContent();
        GetParameter? getParemeter = filterElement.GetGetParameter();
        category.GetParameterNames.Add(getParemeter.Name);
        return new Filter(filterName, category, getParemeter, FilterType.CheckBox);
    }
}
