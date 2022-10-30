using TgBotForSearchWork.Managers.FileManagers;
using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.Managers.UserManagers;

public class UserManager
{
    private readonly FileManager _fileManager;
    private readonly Dictionary<long, User> _users;
    public IEnumerable<User> Users { get => _users.Values; }

    public UserManager(FileManager fileManager)
    {
        _fileManager = fileManager;
        _users = _fileManager.Read().ToDictionary(e=> e.ChatId);
    }

    public void AddUser(long chatId)
    {
        User user = new User(chatId);
        if (_users.TryAdd(chatId, user))
        {
            _fileManager.Write(user);
        }
    }

    public void AddDefaultUser()
    {
        long chatId = 692816611;
        if (_users.ContainsKey(chatId) is false)
        {
            User user = new User(chatId);
            user.UrlsToVacancies.Add(new Url(@"https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=0-1", true), null);
            user.UrlsToVacancies.Add(new Url(@"https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=1-3", true), null);
            user.UrlsToVacancies.Add(new Url(@"https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=1-3", true), null);
            user.UrlsToVacancies.Add(new Url(@"https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=0-1", true), null);
            user.UrlsToVacancies.Add(new Url(@"https://djinni.co/jobs/?employment=remote&primary_keyword=C%2B%2B&exp_level=no_exp&exp_level=1y", true), null);
            user.UrlsToVacancies.Add(new Url(@"https://djinni.co/jobs/?primary_keyword=.NET&exp_level=1y&exp_level=2y&employment=remote", true), null);
            user.UrlsToVacancies.Add(new Url(@"https://www.work.ua/ru/jobs-remote-.net+developer/", true), null);
            user.UrlsToVacancies.Add(new Url(@"https://www.work.ua/ru/jobs-remote-c%2B%2B+developer/", true), null);
            _users.Add(chatId, user);
            _fileManager.Write(user);
        }
    }

    public void RemoveUser(long chatId)
    {
        if (_users.Remove(chatId))
        {
            _fileManager.Write(_users.Values);
        }
    }
}
