﻿using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Extensions;
using Parsers.Models;
using Parsers.Utilities;

namespace Parsers.FilterParsers.Parsers;

internal class WorkUaFilterParser : FilterParser
{
    protected override string SearchGetParamName { get; } = "search";

    protected HtmlElement _filterLink = new("filter-link catlink");
    protected HtmlElement _filterGroup = new("form-group");
    protected HtmlElement _filterGroupTitle = new("h5");
    protected HtmlElement _filterSmallText = new("small");
    protected HtmlElement _input = new(string.Empty, "input");
    protected HtmlElement _option = new(string.Empty, "option");
    protected HtmlElement _div = new(string.Empty, "div");
    protected HtmlElement _span = new(string.Empty, "SPAN");

    protected override void CollectFilters(IDocument document, List<Filter> filters)
    {
        CollectFiltersFromCities(filters);
        IHtmlCollection<IElement> filtersGroup = document.GetElementsByClassName(_filterGroup.CssClassName);
        for (int i = 1; i < filtersGroup.Length; i++)
            CollectFiltersFromFilterGroup(filtersGroup[i], filters);
    }

    protected void CollectFiltersFromFilterGroup(IElement filterGroup, List<Filter> filters)
    {
        string? categoryName = filterGroup.GetElement(_filterGroupTitle)?.TextContent;
        if (categoryName.IsNullOrEmpty())
            return;
        int categoryId = UniqueIntGenerator.Generate();
        List<IElement> elementsWithId = filterGroup.GetElementsWithId(_div);
        
        if (categoryName == "Зарплата")
            CollectFiltersFromSalaryCategory(filterGroup, filters, categoryName!, elementsWithId);
        else
            for (int i = 0; i < elementsWithId.Count; i++)
                CollectFiltersFromElement(filters, elementsWithId[i], categoryId, categoryName!);
    }

    protected void CollectFiltersFromSalaryCategory(IElement filterGroup, List<Filter> filters,
                                              string categoryName, List<IElement> elementsWithId)
    {
        List<IElement> subCategories = filterGroup.GetElements(_filterSmallText);
        for (int i = 0; i < elementsWithId.Count; i++)
        {
            IElement? subCategory = subCategories.ElementAtOrDefault(i);
            string newCategoryName = categoryName + " ";
            if (subCategory is not null)
                newCategoryName += subCategory.TextContent;
            else
                newCategoryName += elementsWithId[i].GetElement(_span)!.TextContent.ToLower();
            CollectFiltersFromElement(filters, elementsWithId[i], UniqueIntGenerator.Generate(), newCategoryName);
        }
    }

    protected void CollectFiltersFromElement(List<Filter> filters, IElement element, int categoryId, string categoryName)
    {
        List<IElement> inputs = element.GetElements(_input);
        bool isOption = inputs.Any() is false;
        if (isOption)
            inputs = element.GetElements(_option);

        foreach (var input in inputs)
            filters.Add(CreateFilter(categoryId, categoryName, input, element, isOption));
    }

    protected Filter CreateFilter(int categoryId, string categoryName, IElement input, IElement elementWithId, bool isOption)
    {
        string? filterName = null;
        if(isOption is true)
            filterName = input.GetTextContent().GetWithoutTextInBrackets();
        else
            filterName = input.GetNearestSiblingTextContent().GetWithoutTextInBrackets();
        string getParamName = elementWithId.Id!.Replace("_selection", string.Empty);
        FilterCategory category = new(categoryId, categoryName, getParamName);
        GetParameter getParameter = new(category.GetParameterName, input.GetValueAttribute());
        return new Filter(filterName, category, getParameter, FilterType.CheckBox);
    }

    protected void CollectFiltersFromCities(List<Filter> filters)
    {
        FilterCategory category = new("Міста", "region");
        string[] cities = { "Вінниця" , "Дніпро", "Донецьк", "Житомир", "Запоріжжя", "Івано-Франківськ","Київ",
                            "Кропивницький", "Сімферополь", "Луганськ", "Луцьк", "Львів",  "Миколаїв", "Одеса",
                            "Полтава","Рівне","Суми", "Тернопіль", "Ужгород", "Харків","Херсон", "Хмельницький",
                            "Черкаси","Чернігів","Чернівці","Інші країни", "Дистанційно" };
        int id = 60;
        for (int i = cities.Length - 1; i >= 0; i--, id--)
        {
            GetParameter getParameter = new(category.GetParameterName, id.ToString());
            filters.Add(new Filter(cities[i], category, getParameter, FilterType.CheckBox));
        }
            
    }

}