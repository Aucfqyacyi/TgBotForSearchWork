using System.IO;

namespace TgBotForSearchWork.Core.FileManagers;

internal class FileWriter : IDisposable
{
    private readonly FileStream _fileStream;
    private readonly StreamWriter _streamWriter;

    public long FileLenght { get => _fileStream.Length; }

    public FileWriter(string path, FileMode fileMode = FileMode.Append)
    {
        _fileStream = File.Open(path, fileMode);
        _streamWriter = new StreamWriter(_fileStream);
    }

    public void WriteLine(long value)
    {
        _streamWriter.WriteLine(value);
    }

    public void WriteLine(string? value)
    {
        
        _streamWriter.WriteLine(value);
    }

    public void WriteLine()
    {
        _streamWriter.WriteLine();
    }

    public void Dispose()
    {
        _streamWriter.Close();
        _fileStream.Close();
        _streamWriter.Dispose();
        _fileStream.Dispose();
    }
}
