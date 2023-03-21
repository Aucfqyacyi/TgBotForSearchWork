using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;
using Parsers.Utilities;

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
        if (categoryName.IsNullOrEmpty())
            return;
        int categoryId = UniqueIntGenerator.Generate();
        foreach (var filterElement in filterElements)
            filters.Add(CreateFilter(categoryId, categoryName!, filterElement));
    }

    protected Filter CreateFilter(int categoryId, string categoryName, IElement filterElement)
    {
        string filterName = filterElement.GetTextContent();
        GetParameter? getParemeter = filterElement.GetGetParameter();
        FilterCategory category = new(categoryId, categoryName, getParemeter.Name);
        return new Filter(filterName, category, getParemeter, FilterType.CheckBox);
    }
}
