using MongoDB.Driver;
using System.Text;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWork.Services;

internal class MongoDbService
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

    public void UpdateUser(User user, CancellationToken cancellationToken)
    {
        ReplaceOptions? replaceOptions = null;
        MongoDb.UserCollection.ReplaceOne(u => u.ChatId == user.ChatId, user, replaceOptions, cancellationToken);
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

    public void AddUrlToUser(long chatId, UrlToVacancies urlToVacancies, CancellationToken cancellationToken)
    {
        User? user = GetUserOrDefault(chatId, cancellationToken);
        if(user == null)
            return;
        user.AddUrlToVacancias(urlToVacancies);
        UpdateUser(user, cancellationToken);
    }
}
