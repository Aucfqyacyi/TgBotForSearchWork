namespace TgBotForSearchWork.Others;

internal static class Log
{
    public static void Info(string message)
    {
        Console.Write(DateTime.Now);
        Console.Write(": ");
        Console.WriteLine(message);
    }
}
