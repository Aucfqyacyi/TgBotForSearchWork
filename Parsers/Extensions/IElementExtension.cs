using AngleSharp.Dom;
using Parsers.Models;
using System.Text;

namespace Parsers.Extensions;

internal static class IElementExtension
{
    private const char _space = ' ';
    private static readonly Dictionary<char, char> _badSymbolsToCorrects = new()
    {
        { '`', '\'' }, { '_', _space }, { '*', _space }, { '\r', _space }
    };

    public static string GetTextContent(this IElement element, HtmlElement htmlElement, int? maxLenght = null)
    {
        return element.GetElement(htmlElement)?.GetTextContent(maxLenght) ?? string.Empty;
    }

    public static string GetTextContent(this IElement element, int? maxLength = null)
    {
        string innerHtml = element!.InnerHtml.Trim('\t', '\n', '\r', ' ');
        StringBuilder stringBuilder = new StringBuilder(innerHtml.Length);
        bool isTag = false;
        maxLength ??= int.MaxValue;
        for (int i = 0; i < innerHtml.Length && i < maxLength; i++)
        {
            if (innerHtml[i] == '<')
            {
                isTag = true;
            }
            else if (innerHtml[i] == '>')
            {
                isTag = false;
            }
            else if (isTag)
            {
                OnTag(innerHtml, i, stringBuilder);
            }
            else if (_badSymbolsToCorrects.ContainsKey(innerHtml[i]))
            {
                stringBuilder.Append(_badSymbolsToCorrects[innerHtml[i]]);
            }
            else if ((innerHtml[i] == _space && innerHtml[i + 1] == _space) ||
                     (innerHtml[i] == '\t' && innerHtml[i + 1] == '\t') ||
                     (innerHtml[i] == '\n' && innerHtml[i + 1] == '\n'))
            {
                i += 1;
            }
            else if (innerHtml[i] == '&')
            {
                i += 5;
                stringBuilder.Append(_space);
            }
            else
            {
                stringBuilder.Append(innerHtml[i]);
            }
        }
        return stringBuilder.ToString().TrimEnd();
    }

    public static void OnTag(string innerHtml, int index, StringBuilder stringBuilder)
    {
        if ((innerHtml[index] == 'b' && innerHtml[index + 1] == 'r') ||
            (innerHtml[index] == 'p' && innerHtml[index - 1] == '/') ||
            (innerHtml[index] == 'u' && innerHtml[index - 1] == '/'))
        {
            stringBuilder.AppendLine();
        }
        else if((innerHtml[index] == 'b' && (innerHtml[index + 1] == '>' || innerHtml[index - 1] == '/')) ||
               ((innerHtml[index] == 'h' && (innerHtml[index - 1] == '<' || innerHtml[index - 1] == '/'))))
        {
            stringBuilder.Append('*');
            if (innerHtml[index] == 'h' && innerHtml[index - 1] == '/')
                stringBuilder.AppendLine();
        }
        else if(innerHtml[index] == 'l' && innerHtml[index - 1] == '<')
        {
            stringBuilder.Append("---");
            stringBuilder.Append(_space);
        }
        else if (innerHtml[index] == 'l' && innerHtml[index - 1] == '/')
        {
            stringBuilder.AppendLine();
        }

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

}

