using System.Security.Cryptography;
using System.Text;

namespace TgBotForSearchWork.Extensions;

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

    public static string GetMD5(this string text)
    {
        StringBuilder sb = new();

        using MD5 hash = MD5.Create();
        Encoding enc = Encoding.ASCII;
        var a = enc.GetBytes(text);
        byte[] result = hash.ComputeHash(enc.GetBytes(text));

        foreach (byte b in result)
        {
            sb.Append(b.ToString("x1"));
        }
        return sb.ToString();
    }

}
