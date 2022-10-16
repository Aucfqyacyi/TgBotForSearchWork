using AngleSharp.Dom;
using TgBotForSearchWork.Constants;
using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.Extensions;

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

    public static string GetHrefAttribute(this IElement element, string host)
    {
        string? url = element.GetAttribute("href");
        if (url is null)
        {
            return string.Empty;
        }
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
        IHtmlCollection<IElement>? elements = null;

        if (htmlElement.CssClassName.IsNotNullOrEmpty())
            elements = vacancyElement.GetElementsByClassName(htmlElement.CssClassName);

        if (htmlElement.TagName.IsNotNullOrEmpty() && (elements is null || elements.Count() == 0))
            elements = vacancyElement.GetElementsByTagName(htmlElement.TagName);

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

}
