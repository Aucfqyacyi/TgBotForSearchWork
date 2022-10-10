namespace TgBotForSearchWork.VacancyParsers.Extensions;

internal static class StringExtension
{
    public static string FormatHtmlText(this string str)
    {
        return str.Trim('\n', '\t', ' ').Replace("&nbsp;", " ").Replace('_', ' ');
    }
}
