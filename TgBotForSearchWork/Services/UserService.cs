using MongoDB.Driver;
using Newtonsoft.Json;
using TgBotForSearchWork.Models;
using TgBotForSearchWork.Utilities;

namespace TgBotForSearchWork.Services;

internal class UserService
{
    public async Task AddUserAsync(long chatId, CancellationToken cancellationToken, IReadOnlyList<string>? urls = null)
    {
        if (await GetUserOrDefaultAsync(chatId, cancellationToken) == null)
        {
            User user = new User(chatId, urls);
            var str = JsonConvert.SerializeObject(user.Urls);
            MongoDb.GetCollection<User>().InsertOne(user, null, cancellationToken);
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
        AddUserAsync(chatId, default, urls).GetAwaiter().GetResult();
    }

    public Task RemoveUserAsync(long chatId, CancellationToken cancellationToken)
    {
        return MongoDb.GetCollection<User>().FindOneAndDeleteAsync(user => user.ChatId == chatId, null, cancellationToken);
    }

    public Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        return MongoDb.GetCollection<User>().FindOneAndReplaceAsync(u => u.ChatId == user.ChatId, user, null, cancellationToken);
    }

    public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await (await MongoDb.GetCollection<User>().FindAsync(e=> e.ChatId != null, null, cancellationToken)).ToListAsync(cancellationToken);
    }

    public  async Task<User?> GetUserOrDefaultAsync(long chatId, CancellationToken cancellationToken)
    {
        return await (await MongoDb.GetCollection<User>().FindAsync(e => e.ChatId == chatId, null, cancellationToken)).FirstOrDefaultAsync(cancellationToken);
    }
}
