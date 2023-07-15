using AngleSharp.Dom;
using Parsers.Models;

namespace Parsers.Extensions;

internal static class IElementExtension
{
    public static string GetTextContent(this IElement element, HtmlElement htmlElement, uint? maxLenght = null)
    {
        return element.GetElement(htmlElement)?.GetTextContent(maxLenght) ?? string.Empty;
    }

    public static string GetTextContent(this IElement element, uint? maxLength = null)
    {
        return element!.InnerHtml.Trim('\t', '\n', '\r', ' ').ParseHtml(maxLength);
    }

    public static string GetNearestSiblingTextContent(this IElement element)
    {
        if (element.NextElementSibling != null)
            return element.NextElementSibling.GetTextContent();
        if (element.PreviousElementSibling != null)
            return element.PreviousElementSibling.GetTextContent();
        return element.GetTextContent();
    }

    public static string GetHrefAttribute(this IElement element)
    {
        return element.GetNotNullAttribute("href");
    }

    public static string GetValueAttribute(this IElement element)
    {
        return element.GetNotNullAttribute("value");
    }

    public static string GetNameAttribute(this IElement element)
    {
        return element.GetNotNullAttribute("name");
    }
    
    public static string GetNotNullAttribute(this IElement element, string attributeName)
    {
        return element.GetAttribute(attributeName) ?? string.Empty;
    }
    
    public static string GetHrefAttribute(this IElement element, string host)
    {
        string url = GetHrefAttribute(element);
        if (url.StartsWith(Uri.UriSchemeHttps + Uri.SchemeDelimiter))
        {
            return url;
        }
        return Uri.UriSchemeHttps + Uri.SchemeDelimiter + host + url;
    }

    public static IElement? GetElement(this IElement iElement, HtmlElement htmlElement)
    {
        List<IElement> elements = iElement.GetElements(htmlElement);

        if (elements.Count > 0)
        {
            foreach (var element in elements)
            {
                if ((element.ClassName.NullableContains(htmlElement.CssClassName) || htmlElement.CssClassName.IsNullOrEmpty()) &&
                    (element.TagName.NullableContains(htmlElement.TagName) || htmlElement.TagName.IsNullOrEmpty()))
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
                        if (elementChild!.Id.IsNotNullOrEmpty())
                            elementsWithId.Add(elementChild);
                    } while ((elementChild = elementChild?.NextElementSibling) != null);
                }
            }
        }
        return elementsWithId;
    }

    public static GetParameter GetGetParameter(this IElement iElement)
    {
        string[] splitGetParameter = iElement.GetHrefAttribute().Split('?').Last().Split('=');
        string getParameterName = splitGetParameter.First();
        if (splitGetParameter.Length == 1)
            return new GetParameter(getParameterName, "1");
        else
            return new GetParameter(getParameterName, splitGetParameter.Last());
    }
}

