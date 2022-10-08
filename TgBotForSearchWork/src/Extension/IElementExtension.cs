using AngleSharp.Dom;

namespace TgBotForSearchWork.src.Extensions;

internal static class IElementExtension
{
    public static string GetFirstChildInnerHtml(this IElement element)
    {
        if (element.FirstElementChild is not null)
        {
            return GetFirstChildInnerHtml(element.FirstElementChild);
        }
        return element.InnerHtml.ReplaceNonBreakingSpace();
    }

    public static string GetHref(this IElement element, string host)
    {
        string http = @"https";
        string? url = element.GetAttribute("href");
        if (url is null)
        {
            return string.Empty;
        }
        if (url.StartsWith(http))
        {
            return url;
        }
        return host + url;
    }
}
