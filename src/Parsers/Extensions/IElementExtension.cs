using AngleSharp.Dom;
using Parsers.Models;

namespace Parsers.Extensions;

internal static class IElementExtension
{
    public static string GetTextContent(this IElement element, HtmlElement htmlElement, int? maxLenght = null)
    {
        return element.GetElement(htmlElement)?.GetTextContent(maxLenght) ?? string.Empty;
    }

    public static string GetTextContent(this IElement element, int? maxLength = null)
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

    public static string GetHrefAttribute(this IElement element, string host)
    {
        string? url = GetHrefAttribute(element);
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
        string[] splitedGetParamater = iElement.GetHrefAttribute().Split('?').Last().Split('=');
        string getParameterName = splitedGetParamater.First();
        if (splitedGetParamater.Length == 1)
            return new GetParameter(getParameterName, "1");
        else
            return new GetParameter(getParameterName, splitedGetParamater.Last());
    }
}

