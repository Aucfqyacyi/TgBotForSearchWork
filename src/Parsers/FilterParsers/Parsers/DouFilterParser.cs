using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;
using Parsers.Utilities;

namespace Parsers.FilterParsers.Parsers;


internal class DouFilterParser : FilterParser
{
    protected override string SearchGetParamName { get; } = "search";

    protected readonly HtmlElement _filterRegion = new("b-region-filter");
    protected readonly HtmlElement _ul = new(string.Empty, "ul");
    protected readonly HtmlElement _a = new(string.Empty, "a");
    protected readonly HtmlElement _category = new(string.Empty, "option");

    public DouFilterParser(HttpClient httpClient) : base(httpClient)
    {
    }

    protected override void CollectFilters(IDocument document, List<Filter> filters)
    {
        FilterCategory category = new("Категорії", "category");
        CollectFiltersFromCategories(document, filters, category);
        CollectFiltersFromFilterRegion(document, filters);
    }

    protected void CollectFiltersFromCategories(IDocument document, List<Filter> filters, FilterCategory category)
    {
        IHtmlCollection<IElement> categories = document.GetElementsByTagName(_category.TagName);
        foreach (var categoryElement in categories)
        {
            Filter? filter = CreateFilterFromCategory(categoryElement, category);
            if (filter is not null)
                filters.Add(filter);
        }
    }

    protected Filter? CreateFilterFromCategory(IElement filterElement, FilterCategory category)
    {
        string filterName = filterElement.GetTextContent();
        string getParameterValue = filterElement.GetValueAttribute();
        if (getParameterValue.IsNullOrEmpty())
            return null;
        GetParameter getParameter = new(category.GetParameterName, getParameterValue);
        return new Filter(filterName, category, getParameter, FilterType.CheckBox);
    }

    protected void CollectFiltersFromFilterRegion(IDocument document, List<Filter> filters)
    {
        IElement? filterRegion = document.GetElementsByClassName(_filterRegion.CssClassName).FirstOrDefault();
        if (filterRegion is null)
            return;

        List<IElement> uls = filterRegion.GetElements(_ul);
        if (uls is null)
            return;

        foreach (var ul in uls)
            CollectFiltersFromFilterRegion(ul, filters);
    }

    protected void CollectFiltersFromFilterRegion(IElement ul, List<Filter> filters)
    {
        string? categoryName = ul.PreviousElementSibling?.GetTextContent();
        if (categoryName.IsNullOrEmpty())
            return;

        int categoryId = UniqueIntGenerator.Generate();
        List<IElement> lis = ul.GetElements(_a);
        if (lis is null)
            return;

        for (int i = 0; i < lis.Count; i++)
            filters.Add(CreateFilterFromFilterRegion(categoryId, categoryName!, lis[i]));
    }

    protected Filter CreateFilterFromFilterRegion(int categoryId, string categoryName, IElement filterElement)
    {
        string filterName = filterElement.GetTextContent();
        GetParameter getParameter = filterElement.GetGetParameter();
        if (getParameter.Value.StartsWith('%'))
            getParameter.Value = filterName;
        FilterCategory category = new(categoryId, categoryName, getParameter.Name);
        return new Filter(filterName, category, getParameter, FilterType.CheckBox);
    }


}
