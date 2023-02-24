using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;

namespace Parsers.FilterParsers.Parsers;


internal class DouFilterParser : FilterParser
{
    protected override string SearchGetParamName { get; } = "search";

    protected readonly HtmlElement _filterRegion = new("b-region-filter");
    protected readonly HtmlElement _ul = new(string.Empty, "ul");
    protected readonly HtmlElement _li = new(string.Empty, "li");
    protected readonly HtmlElement _category = new("", "option");

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
            if(filter is not null)
                filters.Add(filter);
        }
            
    }

    protected Filter? CreateFilterFromCategory(IElement filterElement, FilterCategory category)
    {
        string filterName = filterElement.GetTextContent();
        string filterGetParamater = filterElement.GetValueAttribute();
        if (filterGetParamater.IsNullOrEmpty())
            return null;
        return new(filterName, category, filterGetParamater, FilterType.CheckBox);
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
        if (categoryName is null)
            return;
        FilterCategory category = new(categoryName);
        List<IElement> lis = ul.GetElements(_li);
        if (lis is null)
            return;

        for (int i = 1; i < lis.Count; i++)
            filters.Add(CreateFilterFromFilterRegion(lis[i], category));
    }

    protected Filter CreateFilterFromFilterRegion(IElement filterElement, FilterCategory category)
    {
        string filterName = filterElement.GetTextContent();
        string[] splitedGetParamater = filterElement.FirstElementChild!.GetHrefAttribute().Split('?').Last().Split('=');
        category.GetParameterName ??= splitedGetParamater.First();
        return new(filterName, category, splitedGetParamater.Last(), FilterType.CheckBox);
    }


}
