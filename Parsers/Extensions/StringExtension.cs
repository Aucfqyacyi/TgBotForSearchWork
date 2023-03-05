using System.Text;
using System.Web;

namespace Parsers.Extensions;

public static class StringExtension
{
    public static bool IsNotNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str) is false;
    }

    public static bool IsNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    public static bool NullableContains(this string? str, string value)
    {
        if (str.IsNullOrEmpty())
            return false;
        return str!.Contains(value);
    }

    public static string GetWithoutTextInBrackets(this string? str)
    {
        if (str == null)
            return string.Empty;
        return str.Split('(').First().TrimEnd();
    }

    public static string ParseHtml(this string htmlText, int? maxLength = null)
    {
        StringBuilder stringBuilder = new StringBuilder(htmlText.Length);
        bool isTag = false;
        maxLength ??= int.MaxValue;
        for (int i = 0; i < htmlText.Length && (i < maxLength || isTag); i++)
        {
            if (htmlText[i] == '<')
            {
                isTag = true;
            }
            else if (htmlText[i] == '>')
            {
                isTag = false;
            }
            else if (isTag)
            {
                OnTag(htmlText, i, stringBuilder);
            }
            else if ((htmlText[i] == ' ' && htmlText[i + 1] == ' ') ||
                     (htmlText[i] == '\t' && htmlText[i + 1] == '\t') ||
                     (htmlText[i] == '\n' && htmlText[i + 1] == '\n') ||
                      htmlText[i] == '\r')
            {
                i += 1;
            }
            else
            {
                stringBuilder.Append(htmlText[i]);
            }
        }
        return HttpUtility.HtmlDecode(stringBuilder.ToString().TrimEnd());
    }

    private static void OnTag(string htmlText, int index, StringBuilder stringBuilder)
    {
        if ((htmlText[index] == 'b' && htmlText[index + 1] == 'r') ||
            (htmlText[index] == 'p' && htmlText[index - 1] == '/') ||
            (htmlText[index] == 'u' && htmlText[index - 1] == '/') ||
             htmlText[index] == 'l' && htmlText[index - 1] == '/')
        {
            stringBuilder.AppendLine();
        }
        else if (htmlText[index] == 'h' && (htmlText[index - 1] == '<' || htmlText[index - 1] == '/'))
        {
            if (htmlText[index] == 'h' && htmlText[index - 1] == '<')
                stringBuilder.Append("<b>");
            else
                stringBuilder.AppendLine("</b>");
        }
        else if (htmlText[index] == 'l' && htmlText[index - 1] == '<')
        {
            stringBuilder.Append("--- ");
        }
    }

    public static ulong GetNumberFromUrl(this string url, uint numberPositionInUrl, string? symbolNearNumber = null)
    {
        string rawNumber = url.Split('/')[numberPositionInUrl];
        string? number = null;
        if (symbolNearNumber is not null)
            number = rawNumber.Split(symbolNearNumber).First();
        else
            number = rawNumber;
        
        if (ulong.TryParse(number, out ulong num))
            return num;
        else
            throw new Exception("String doesn't contain the number.");
    }
}
