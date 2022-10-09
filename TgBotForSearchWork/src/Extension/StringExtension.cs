namespace TgBotForSearchWork.src.Extensions;

internal static class StringExtension
{
    public static string FormatHtmlText(this string str)
    {
        return str.Trim('\n', '\t', ' ').Replace("&nbsp;", " ");
    }

    public static bool IsNotNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str) is false;
    }
}
