namespace TgBotForSearchWorkApi.Utilities;

internal static class Log
{
    public static void Info(string message)
    {
        Console.WriteLine($"{DateTime.Now}: {message}");
    }
}
