using MongoDB.Driver;
using TgBotForSearchWork.Models;
using TgBotForSearchWorkApi.Utilities;
using TgBotForSearchWorkApi.Utilities.Attributes;

namespace TgBotForSearchWork.Services;

[SingletonService]
public class UserRepository
{
    private readonly MongoDbContext _mongoContext;

    public UserRepository(MongoDbContext mongoContext)
    {
        _mongoContext = mongoContext;
    }

    public User Add(long chatId, CancellationToken cancellationToken)
    {
        User? user = GetOrDefault(chatId, cancellationToken);
        if (user is null)
        {
            user = new User(chatId);
            _mongoContext.UserCollection.InsertOne(user, null, cancellationToken);
        }
        return user;
    }

    public void AddDefaultUser()
    {
        long chatId = 692816611;
        User user = Add(chatId, default);
        if (user.Urls.Count > 0)
            return;
        user.AddUrlToVacancias(new(@"https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=1-3"));
        user.AddUrlToVacancias(new(@"https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=1-3"));
        user.AddUrlToVacancias(new(@"https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=0-1"));
        user.AddUrlToVacancias(new(@"https://djinni.co/jobs/?employment=remote&primary_keyword=C%2B%2B&exp_level=no_exp&exp_level=1y"));
        user.AddUrlToVacancias(new(@"https://djinni.co/jobs/?primary_keyword=.NET&exp_level=1y&exp_level=2y&employment=remote"));
        user.AddUrlToVacancias(new(@"https://www.work.ua/ru/jobs-remote-.net+developer/"));
        user.AddUrlToVacancias(new(@"https://www.work.ua/ru/jobs-remote-c%2B%2B+developer/"));
        Update(user, default);
    }

    public void Remove(long chatId, CancellationToken cancellationToken)
    {
        _mongoContext.UserCollection.DeleteOne(user => user.ChatId == chatId, null, cancellationToken);
    }

    public void Update(User user, CancellationToken cancellationToken)
    {
        ReplaceOptions? replaceOptions = null;
        _mongoContext.UserCollection.ReplaceOne(u => u.ChatId == user.ChatId, user, replaceOptions, cancellationToken);
    }

    public List<User> GetAll(CancellationToken cancellationToken)
    {
        return _mongoContext.UserCollection.FindSync(e => e.ChatId != null, null, cancellationToken)
                                     .ToList(cancellationToken);
    }

    public User? GetOrDefault(long chatId, CancellationToken cancellationToken)
    {
        return _mongoContext.UserCollection.FindSync(e => e.ChatId == chatId, null, cancellationToken)
                                     .FirstOrDefault(cancellationToken);
    }

    public User Get(long chatId, CancellationToken cancellationToken)
    {
        User? user = GetOrDefault(chatId, cancellationToken);
        if (user is null)
            throw new Exception("User does not exist.");
        return user;
    }
}
