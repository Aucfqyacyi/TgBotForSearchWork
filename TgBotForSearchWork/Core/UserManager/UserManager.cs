using TgBotForSearchWork.Core.FileManagers;

namespace TgBotForSearchWork.Core.UserManagers;

public class UserManager
{
    private readonly FileManager _fileManager;
    private readonly List<User> _users;
    public IEnumerable<User> Users { get => _users; }
    public UserManager(FileManager fileManager)
    {
        _fileManager = fileManager;
        _users = _fileManager.Read();
    }

    public void AddUser(long chatId)
    {
        if (_users.Any(user => user.ChatId == chatId) is false)
        {
            User user = new User(chatId);
            user.UrisToVacancies.Add(new Uri(@"https://jobs.dou.ua/vacancies/?remote&category=.NET&exp=1-3"), null);
            user.UrisToVacancies.Add(new Uri(@"https://jobs.dou.ua/vacancies/?remote&category=C%2B%2B&exp=0-1"), null);
            user.UrisToVacancies.Add(new Uri(@"https://djinni.co/jobs/?employment=remote&primary_keyword=C%2B%2B&exp_level=no_exp&exp_level=1y"), null);
            user.UrisToVacancies.Add(new Uri(@"https://djinni.co/jobs/?primary_keyword=.NET&exp_level=1y&exp_level=2y&employment=remote"), null);
            user.UrisToVacancies.Add(new Uri(@"https://www.work.ua/ru/jobs-remote-.net+developer/"), null);
            user.UrisToVacancies.Add(new Uri(@"https://www.work.ua/ru/jobs-remote-c%2B%2B+developer/"), null);
            _users.Add(user);
            _fileManager.Write(user);
        }
    }

    public void RemoveUser(long chatId)
    {
        User? user = _users.FirstOrDefault(user => user.ChatId == chatId);
        if (user is not null)
        {
            _users.Remove(user);
            _fileManager.Write(_users);
        }
    }
}
