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
        CollectFiltersFromCategories(document, filters);
        CollectFiltersFromFilterRegion(document, filters);
    }

    protected void CollectFiltersFromCategories(IDocument document, List<Filter> filters)
    {
        IHtmlCollection<IElement> categories = document.GetElementsByTagName(_category.TagName);
        foreach (var categoryElement in categories)
        {
            Filter? filter = CreateFilterFromCategory(categoryElement);
            if(filter is not null)
                filters.Add(filter);
        }
            
    }

    protected Filter? CreateFilterFromCategory(IElement filterElement)
    {
        string filterName = filterElement.GetFirstChildTextContent();
        string filterGetParamater = filterElement.GetValueAttribute();
        if (filterGetParamater.IsNullOrEmpty())
            return null;
        return new(filterName, "Категорії", "category", filterGetParamater, FilterType.CheckBox);
    }

    protected void CollectFiltersFromFilterRegion(IDocument document, List<Filter> filters)
    {
        IElement? filterRegion = document.GetElementsByClassName(_filterRegion.CssClassName).FirstOrDefault();
        if (filterRegion == null)
            return;

        List<IElement> uls = filterRegion.GetElements(_ul);
        if (uls == null)
            return;

        foreach (var ul in uls)
            CollectFiltersFromFilterRegion(ul, filters);
    }

    protected void CollectFiltersFromFilterRegion(IElement ul, List<Filter> filters)
    {
        string category = ul.PreviousElementSibling!.GetFirstChildTextContent();
        List<IElement> lis = ul.GetElements(_li);
        if (lis == null)
            return;

        for (int i = 1; i < lis.Count; i++)
            filters.Add(CreateFilterFromFilterRegion(lis[i], category));
    }

    protected Filter CreateFilterFromFilterRegion(IElement filterElement, string category)
    {
        string filterName = filterElement.GetFirstChildTextContent();
        string[] splitedGetParamater = filterElement.FirstElementChild!.GetHrefAttribute().Split('?').Last().Split('=');
        return new(filterName, category, splitedGetParamater.First(), splitedGetParamater.Last(), FilterType.CheckBox);
    }


}
