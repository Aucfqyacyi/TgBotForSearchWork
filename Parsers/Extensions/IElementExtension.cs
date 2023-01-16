using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Models;

namespace Parsers.Extensions;

internal static class IElementExtension
{
    public static string GetTextContent(this IElement element, HtmlElement htmlElement)
    {
        return element.GetElement(htmlElement)?.GetFirstChildTextContent() ?? string.Empty;
    }

    public static string GetFirstChildTextContent(this IElement element)
    {
        if (element?.FirstElementChild != null && element.FirstElementChild.TagName != "BR")
        {
            return element.FirstElementChild.GetFirstChildTextContent();
        }
        return element!.TextContent.Trim('\t', '\n', ' ').Replace("  ", string.Empty).Replace('_', ' ') + '\n';
    }

    public static string GetNearestSiblingTextContent(this IElement element)
    {
        if (element.NextElementSibling != null)
            return element.NextElementSibling.GetFirstChildTextContent();
        if(element.PreviousElementSibling != null)
            return element.PreviousElementSibling.GetFirstChildTextContent();
        return element.GetFirstChildTextContent();
    }

    public static string GetHrefAttribute(this IElement element)
    {
        string? url = element.GetAttribute("href");
        if (url == null)
        {
            return string.Empty;
        }
        return url;
    }

    public static string GetValueAttribute(this IElement element)
    {
        string? url = element.GetAttribute("value");
        if (url == null)
        {
            return string.Empty;
        }
        return url;
    }

    public static string GetParametersFromHrefAttribute(this IElement element)
    {
        string hrefAttribute = GetHrefAttribute(element);
        if(hrefAttribute.IsNullOrEmpty())
            return string.Empty;
        return hrefAttribute.Split('?')[1];
    }

    public static string GetHrefAttribute(this IElement element, string host)
    {
        string? url = GetHrefAttribute(element);
        if (url.StartsWith(UrlsToSites.Https))
        {
            return url;
        }
        return UrlsToSites.Https + host + url;
    }

    public static IElement? GetElement(this IElement iElement, HtmlElement htmlElement)
    {
        List<IElement> elements = iElement.GetElements(htmlElement);

        if (elements.Count > 0)
        {
            foreach (var element in elements)
            {
                if ((element.ClassName!.Contains(htmlElement.CssClassName) || string.IsNullOrEmpty(htmlElement.CssClassName)) &&
                    (element.TagName == htmlElement.TagName || string.IsNullOrEmpty(htmlElement.TagName)))
                {
                    return element;
                }
            }
        }
        return null;
    }


    public static List<IElement> GetElements(this IElement element, params HtmlElement[] htmlElements)
    {
        List<IElement> elements = new();
        foreach (var htmlElement in htmlElements)
        {
            if (htmlElement.CssClassName.IsNotNullOrEmpty())
                elements.AddRange(element.GetElementsByClassName(htmlElement.CssClassName));
            else
                elements.AddRange(element.GetElementsByTagName(htmlElement.TagName));
        }        
        return elements;
    }

    public static List<IElement> GetElementsWithId(this IElement iElement, params HtmlElement[] htmlElements)
    {
        List<IElement> elementsWithId = new();
        List<IElement> elements = iElement.GetElements(htmlElements);
        if (elements != null)
        {          
            foreach (var element in elements)
            {
                if (element.Id.IsNotNullOrEmpty())
                    elementsWithId.Add(element);
                else
                {
                    IElement? elementChild = element.FirstElementChild;
                    do
                    {
                        if(elementChild!.Id.IsNotNullOrEmpty())
                            elementsWithId.Add(elementChild);
                    } while ((elementChild = elementChild?.NextElementSibling) != null);
                }
            }
        }     
        return elementsWithId;
    }

}
