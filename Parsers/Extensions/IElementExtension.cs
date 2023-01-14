using AngleSharp.Dom;
using Parsers.Constants;
using Parsers.Models;

namespace Parsers.Extensions;

internal static class IElementExtension
{
    public static string GetFirstChildTextContent(this IElement element)
    {
        if (element.FirstElementChild is not null && element.FirstElementChild.TagName != "BR")
        {
            return element.FirstElementChild.GetFirstChildTextContent();
        }
        return element.TextContent.Trim('\t', '\n', ' ').Replace("  ", string.Empty).Replace('_', ' ') + '\n';
    }

    public static string GetHrefAttribute(this IElement element)
    {
        string? url = element.GetAttribute("href");
        if (url is null)
        {
            return string.Empty;
        }
        return url;
    }

    public static string GetValueAttribute(this IElement element)
    {
        string? url = element.GetAttribute("value");
        if (url is null)
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
        if (url.StartsWith(Host.Https))
        {
            return url;
        }
        return Host.Https + host + url;
    }

    public static string GetTextContent(this IElement vacancyElement, HtmlElement htmlElement)
    {
        return vacancyElement.GetIElement(htmlElement)?.GetFirstChildTextContent() ?? string.Empty;
    }

    public static IElement? GetIElement(this IElement vacancyElement, HtmlElement htmlElement)
    {
        IHtmlCollection<IElement>? elements = vacancyElement.GetIElements(htmlElement);

        if (elements is not null)
        {
            foreach (var element in elements)
            {
                if ((element.ClassName == htmlElement.CssClassName || string.IsNullOrEmpty(htmlElement.CssClassName)) &&
                    (element.TagName == htmlElement.TagName || string.IsNullOrEmpty(htmlElement.TagName)))
                {
                    return element;
                }
            }
        }
        return null;
    }


    public static IHtmlCollection<IElement>? GetIElements(this IElement vacancyElement, HtmlElement htmlElement)
    {
        IHtmlCollection<IElement>? elements = null;

        if (htmlElement.CssClassName.IsNotNullOrEmpty())
            elements = vacancyElement.GetElementsByClassName(htmlElement.CssClassName);

        if (htmlElement.TagName.IsNotNullOrEmpty() && (elements is null || elements.Count() == 0))
            elements = vacancyElement.GetElementsByTagName(htmlElement.TagName);
        return elements;
    }

}
