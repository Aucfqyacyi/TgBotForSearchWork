namespace TgBotForSearchWork.Extensions;

internal static class StringExtension
{
    public static bool IsNotNullOrEmpty(this string? str)
    {
        return string.IsNullOrEmpty(str) is false;
    }
}
