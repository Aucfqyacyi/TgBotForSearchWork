using System.Security.Cryptography;
using System.Text;

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
        if(str == null)
            return false;
        return str.Contains(value);
    }

    public static string GetWithoutTextInBrackets(this string? str)
    {
        if (str == null)
            return string.Empty;
        return str.Split('(').First().TrimEnd();
    }
}
