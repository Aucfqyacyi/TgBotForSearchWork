﻿using TgBotForSearchWork.Core.UserManagers;
using TgBotForSearchWork.Extensions;

namespace TgBotForSearchWork.Core.FileManagers;

public class FileManager
{
    private readonly string _filename;

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
        foreach (var url in user.UrisToVacancies.Keys)
        {
            fileWriter.WriteLine(url.OriginalString);
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
                user.UrisToVacancies.Add(new Uri(line!), null);
            }
            users.Add(user);
        }
        return users;
    }
}