namespace TgBotForSearchWork.src.Extensions;

internal static class StringExtension
{
    public static string ReplaceNonBreakingSpace(this string str)
    {
        return str.Replace("&nbsp;", " ");
    }
}
