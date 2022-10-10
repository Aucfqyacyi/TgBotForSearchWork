using TgBotForSearchWork.Core.Constants;
using TgBotForSearchWork.VacancyParsers.Constants;

namespace TgBotForSearchWork.Core.Other;

internal static class UrlEncryption
{
    private const char _separator = '_';

    public static string Encrypt(Uri uri)
    {
        Site? site = null;
        foreach (var host in Host.All)
        {
            if (host.Value == uri.Host)
            {
                site = host.Key;
                break;
            }
        }
        if (site == null)
            return string.Empty;
        else
            return ((int)site).ToString() + _separator + uri.AbsolutePath;
    }

    public static string Dencrypt(string encryptedUrl)
    {
        int indexOfSeparator = encryptedUrl.IndexOf(_separator) + 1;
        string siteNumber = encryptedUrl.Substring(0, indexOfSeparator);
        if (int.TryParse(siteNumber.TrimEnd(_separator), out int number))
        {
            string url = encryptedUrl.Substring(indexOfSeparator);
            Site site = (Site)number;
            return Host.Https + Host.All[site] + url;
        }
        return string.Empty;
    }
}
