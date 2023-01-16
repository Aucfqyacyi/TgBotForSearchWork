using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;

namespace Parsers.FilterParsers.Parsers;


internal class DjinniFilterParser : FilterParser
{
    protected override string UriToMainPage { get; } = UrlsToSites.GetFullUrlToSite(SiteType.Djinni, "/jobs/");
    protected override string SearchGetParamName { get; } = "keywords";

    protected HtmlElement _filterSet = new("jobs-filter__set");
    protected HtmlElement _filterLink = new("jobs-filter__link", "a");

    protected override void CollectFilters(IDocument doc, List<Filter> filters)
    {
        IHtmlCollection<IElement> sets = doc.GetElementsByClassName(_filterSet.CssClassName);
        foreach (var set in sets)
            CollectFiltersFromSet(set, filters);
    }

    protected void CollectFiltersFromSet(IElement set, List<Filter> filters)
    {       
        List<IElement>? filterElements = set.GetElements(_filterLink);
        if (filterElements == null)
            return;

        string category = set.PreviousElementSibling?.GetFirstChildTextContent() ?? string.Empty;
        foreach (var filterElement in filterElements)
            filters.Add(CreateFilter(filterElement, category));
    }

    protected Filter CreateFilter(IElement filterElement, string category)
    {
        string filterName = filterElement.GetFirstChildTextContent();
        string filterGetParamater = filterElement.GetHrefAttribute().TrimStart('?');
        return new(filterName, category, filterGetParamater, Enums.FilterType.CheckBox);
    }
}
