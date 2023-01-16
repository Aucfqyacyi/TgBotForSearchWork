using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;

namespace Parsers.FilterParsers.Parsers;

internal class WorkUaFilterParser : FilterParser
{
    protected override string SearchGetParamName { get; } = "search";

    protected override string UriToMainPage { get; } = UrlsToSites.GetFullUrlToSite(SiteType.WorkUa,  "/jobs/?advs=1");

    protected HtmlElement _filterLink = new("filter-link catlink");
    protected HtmlElement _filterGroup = new("form-group");
    protected HtmlElement _filterGroupTitle = new("h5");
    protected HtmlElement _input = new(string.Empty, "input");
    protected HtmlElement _option = new(string.Empty, "option");
    protected HtmlElement _div = new(string.Empty, "div");
    protected string _textToCut = "_selection";

    protected override void CollectFilters(IDocument doc, List<Filter> filters)
    {
        CollectFiltersFromCities(filters);
        IHtmlCollection<IElement> filtersGroup = doc.GetElementsByClassName(_filterGroup.CssClassName);
        for (int i = 1; i < filtersGroup.Length; i++)
        {
            CollectFiltersFromFilterGroup(filtersGroup[i], filters, _input, _option);
        }        
    }

    protected void CollectFiltersFromFilterGroup(IElement filterGroup, List<Filter> filters, params HtmlElement[] tags)
    {
        string category = filterGroup.GetElement(_filterGroupTitle)?.GetFirstChildTextContent() ?? string.Empty;       
        List<IElement> elementsWithId = filterGroup.GetElementsWithId(_div);
        foreach (var elementWithId in elementsWithId)
        {
            List<IElement>? inputs = elementWithId.GetElements(tags);
            if (inputs == null)
                continue;
            foreach (var input in inputs)
            {
                filters.Add(CreateFilter(category, input, elementWithId));
            }
        }
    }

    protected Filter CreateFilter(string category, IElement input, IElement elementWithId)
    {
        string filterName = input.GetNearestSiblingTextContent();
        string filterGetParam = elementWithId.Id!.Replace(_textToCut, string.Empty) + "=" + input.GetValueAttribute();
        return new(filterName, category, filterGetParam, Enums.FilterType.CheckBox);
    }

    protected void CollectFiltersFromCities(List<Filter> filters)
    {
        string category = "Міста";
        string getParamName = "region=";
        string[] cities = { "Вінниця" , "Дніпро", "Донецьк", "Житомир", "Запоріжжя", "Івано-Франківськ","Київ",
                            "Кропивницький", "Сімферополь", "Луганськ", "Луцьк", "Львів",  "Миколаїв", "Одеса",
                            "Полтава","Рівне","Суми", "Тернопіль", "Ужгород", "Харків","Херсон", "Хмельницький",
                            "Черкаси","Чернігів","Чернівці","Інші країни", "Дистанційно" };
        int id = 60;
        for (int i = cities.Length - 1; i >=  0; i--, id--)
        {
            filters.Add(new(cities[i], category, getParamName + id, Enums.FilterType.CheckBox));
        }

    }
}
