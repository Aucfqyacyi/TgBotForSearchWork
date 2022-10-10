using AngleSharp.Dom;
using TgBotForSearchWork.Core;

namespace TgBotForSearchWork.Extensions;

internal static class IElementExtension
{
    public static string GetFirstChildInnerHtml(this IElement element)
    {
        if (element.FirstElementChild is not null)
        {
            return GetFirstChildInnerHtml(element.FirstElementChild);
        }
        return element.InnerHtml.FormatHtmlText();
    }

    public static string GetHref(this IElement element, string host)
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
}
