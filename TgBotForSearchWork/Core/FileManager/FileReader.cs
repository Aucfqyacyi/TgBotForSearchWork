using System.IO;

namespace TgBotForSearchWork.Core.FileManagers;

internal class FileReader : IDisposable
{
    private readonly FileStream _fileStream;
    private readonly StreamReader _streamRead;

    public FileReader(string path)
    {
        _fileStream = File.Open(path, FileMode.OpenOrCreate);
        _streamRead = new StreamReader(_fileStream);
    }

    public string? ReadLine()
    {
        return _streamRead.ReadLine();
    }

    public void Dispose()
    {
        _streamRead.Close();
        _fileStream.Close();
        _streamRead.Dispose();
        _fileStream.Dispose();
    }
}
