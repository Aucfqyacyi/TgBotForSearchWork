using MongoDB.Driver;
using System.Text;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWork.Services;

internal class UserService
{
    public void AddUser(long chatId, CancellationToken cancellationToken, IReadOnlyList<string>? urls = null)
    {
        if (GetUserOrDefault(chatId, cancellationToken) == null)
        {
            User user = new User(chatId, urls);
            MongoDb.UserCollection.InsertOne(user, null, cancellationToken);
        }
    }

    public void AddDefaultUser()
    {
        long chatId = 692816611;
        List<string> urls = new();
        urls.Add(@"https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=1-3");
        urls.Add(@"https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=1-3");
        urls.Add(@"https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=0-1");
        urls.Add(@"https://djinni.co/jobs/?employment=remote&primary_keyword=C%2B%2B&exp_level=no_exp&exp_level=1y");
        urls.Add(@"https://djinni.co/jobs/?primary_keyword=.NET&exp_level=1y&exp_level=2y&employment=remote");
        urls.Add(@"https://www.work.ua/ru/jobs-remote-.net+developer/");
        urls.Add(@"https://www.work.ua/ru/jobs-remote-c%2B%2B+developer/");
        AddUser(chatId, default, urls);
    }

    public void RemoveUser(long chatId, CancellationToken cancellationToken)
    {
        MongoDb.UserCollection.DeleteOne(user => user.ChatId == chatId, null, cancellationToken);
    }

    public User UpdateUser(User user, CancellationToken cancellationToken)
    {
        return MongoDb.UserCollection.FindOneAndReplace(u => u.ChatId == user.ChatId, user, null, cancellationToken);
    }

    public List<User> GetAllUsers(CancellationToken cancellationToken)
    {
        return MongoDb.UserCollection.FindSync(e=> e.ChatId != null, null, cancellationToken)
                                     .ToList(cancellationToken);
    }

    public User? GetUserOrDefault(long chatId, CancellationToken cancellationToken)
    {
        return MongoDb.UserCollection.FindSync(e => e.ChatId == chatId, null, cancellationToken)
                                     .FirstOrDefault(cancellationToken);
    }
    public void UpdateLastCommandFieldInUser(long chatId, string command, CancellationToken cancellationToken)
    {
        var userUpdateDefinition = Builders<User>.Update.Set(user => user.LastCommand, command);
        MongoDb.UserCollection.UpdateOne(user => user.ChatId == chatId, userUpdateDefinition, null, cancellationToken);
    }

    public List<string> GetGroupedUrls(long chatId, CancellationToken cancellationToken)
    {
        User? user = GetUserOrDefault(chatId, cancellationToken);       
        if (user == null)
            return new();
        Dictionary<string, StringBuilder> hostsToGroupedUrls = new();
        foreach (UrlToVacancies url in user.Urls)
        {
            StringBuilder? stringBuilder = hostsToGroupedUrls.GetValueOrDefault(url.Host);
            if (stringBuilder == null)
            {
                stringBuilder = new();
                stringBuilder.AppendLine(url.Host+'\n' + url.OriginalString);
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
