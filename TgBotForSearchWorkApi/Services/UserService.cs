using System.Text;
using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.Services;

public class UserService
{
    public List<string> GetGroupedUrls(User user)
    {
        Dictionary<string, StringBuilder> hostsToGroupedUrls = new();
        foreach (UrlToVacancies url in user.Urls)
        {
            StringBuilder? stringBuilder = hostsToGroupedUrls.GetValueOrDefault(url.Host);
            if (stringBuilder == null)
            {
                stringBuilder = new();
                stringBuilder.AppendLine(url.Host + '\n' + url.OriginalString);
                hostsToGroupedUrls.Add(url.Host, stringBuilder);
            }
            else
                stringBuilder.AppendLine(url.OriginalString);
        }
        return hostsToGroupedUrls.Values.Aggregate(new List<string>(), (strings, stringBuilder) =>
        {
            strings.Add(stringBuilder.ToString());
            return strings;
        });
    }
}
