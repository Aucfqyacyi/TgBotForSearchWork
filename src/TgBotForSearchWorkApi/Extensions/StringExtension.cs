﻿using System.Security.Cryptography;
using System.Text;

namespace TgBotForSearchWorkApi.Extensions;

public static class StringExtension
{
    public static string GetMD5(this string text)
    {
        StringBuilder sb = new();

        using MD5 hash = MD5.Create();
        Encoding enc = Encoding.UTF8;
        byte[] result = hash.ComputeHash(enc.GetBytes(text));

        foreach (byte b in result)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
}
