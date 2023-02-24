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
        if (categoryName is null)
            return;
        FilterCategory filterCategory = new(categoryName);
        foreach (var filterElement in filterElements)
            filters.Add(CreateFilter(filterElement, filterCategory));
    }

    protected Filter CreateFilter(IElement filterElement, FilterCategory category)
    {
        string filterName = filterElement.GetTextContent();
        string[] splitedGetParamater = filterElement.GetHrefAttribute().TrimStart('?').Split('=');
        category.GetParameterName ??= splitedGetParamater.First();
        return new(filterName, category, splitedGetParamater.Last(), FilterType.CheckBox);
    }
}
