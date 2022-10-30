using TgBotForSearchWork.Extensions;
using TgBotForSearchWork.Models;

namespace TgBotForSearchWork.Managers.FileManagers;

public class FileManager
{
    private readonly string _filename;
    private char _exclamationMark = '!';

    public FileManager(string filename)
    {
        _filename = filename;
    }

    public void Write(User userInfo)
    {
        using FileWriter fileWriter = new(_filename);
        Write(fileWriter, userInfo);
    }

    public void Write(IEnumerable<User> users)
    {
        using FileWriter fileWriter = new(_filename, FileMode.Truncate);
        foreach (var user in users)
        {
            Write(fileWriter, user);
        }
    }

    private void Write(FileWriter fileWriter, User user)
    {
        fileWriter.WriteLine(user.ChatId);
        foreach (var url in user.UrlsToVacancies.Keys)
        {
            string str = url.OriginalString;
            if (url.IsOff is false)
                str = _exclamationMark + str;
            fileWriter.WriteLine(str);
        }
        fileWriter.WriteLine();
    }

    public List<User> Read()
    {
        using FileReader fileReader = new(_filename);
        List<User> users = new List<User>();
        string? line = null;
        while ((line = fileReader.ReadLine()).IsNotNullOrEmpty())
        {
            User user = new(Convert.ToInt64(line));
            while ((line = fileReader.ReadLine()).IsNotNullOrEmpty())
            {
                if (line!.StartsWith(_exclamationMark))
                {
                    user.UrlsToVacancies.Add(new Url(line!.TrimStart(_exclamationMark)), null);
                }
                else
                {
                    user.UrlsToVacancies.Add(new Url(line!, true), null);
                }
                
            }
            users.Add(user);
        }
        return users;
    }
}
