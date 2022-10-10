using AngleSharp.Dom;
using System.Text;
using TgBotForSearchWork.Extensions;
using TgBotForSearchWork.VacancyParsers.Constants;

namespace TgBotForSearchWork.VacancyParsers.Extensions;

internal static class IElementExtension
{
    public static string GetFirstChildInnerHtml(this IElement element)
    {
        if (element.FirstElementChild is not null)
        {
            return element.FirstElementChild.GetFirstChildInnerHtml();
        }
        return element.InnerHtml.FormatHtmlText();
    }


    public static string GetAllText(this IElement element)
    {
        StringBuilder stringBuilder = new();
        do
        {
            stringBuilder.Append(GetFirstChildInnerHtml(element));

        } while ((element = element?.NextElementSibling) is not null);

        return stringBuilder.ToString();
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

}
