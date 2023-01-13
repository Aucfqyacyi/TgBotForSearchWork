using MongoDB.Driver;
using Newtonsoft.Json;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWork.Services;

public class UserService
{
    private readonly IMongoCollection<User> _usersCollection;

    public UserService()
    {
        _usersCollection = MongoDb.GetCollection<User>();
    }

    public void AddUser(long chatId, IReadOnlyList<string>? urls = null, CancellationToken cancellationToken = default)
    {
        if (!_usersCollection.Find(u => u.ChatId == chatId).ToList().Any())
        {
            User user = new User(chatId, urls);
            var str = JsonConvert.SerializeObject(user.Urls);
            _usersCollection.InsertOne(user, null, cancellationToken);
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
        AddUser(chatId, urls);
    }

    public void RemoveUser(long chatId, CancellationToken cancellationToken = default)
    {
        _usersCollection.FindOneAndDelete(user => user.ChatId == chatId, null, cancellationToken);
    }

    public void UpdateUser(User user, CancellationToken cancellationToken = default)
    {
        _usersCollection.FindOneAndReplace(u => u.ChatId == user.ChatId, user, null, cancellationToken);
    }

    public List<User> GetAllUsers(CancellationToken cancellationToken = default)
    {
        return _usersCollection.Find(e=> e.ChatId != null).ToList(cancellationToken);
    }

    public User? GetUserOrDefault(long chatId, CancellationToken cancellationToken = default)
    {
        return _usersCollection.Find(e => e.ChatId == chatId).ToList(cancellationToken).FirstOrDefault();
    }
}
