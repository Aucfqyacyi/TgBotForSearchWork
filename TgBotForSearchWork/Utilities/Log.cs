namespace TgBotForSearchWork.Others;

internal static class Log
{
    public static void Info(string message)
    {
        Console.WriteLine($"{DateTime.Now}: {message}");
    }
}
