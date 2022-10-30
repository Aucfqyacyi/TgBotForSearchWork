namespace TgBotForSearchWork.Others;

public static class CommandLineArgs
{
    public static readonly string Token;
    public static readonly int Time;

    private const string ArgToken = "--token";
    private const string ArgTime = "--time";

    static CommandLineArgs()
    {
        Dictionary<string, string> commandArgsToValues = GetCommandArgsToValues(Environment.GetCommandLineArgs());
        Token = commandArgsToValues.GetValueOrDefault(ArgToken) ?? string.Empty;
        if (int.TryParse(commandArgsToValues.GetValueOrDefault(ArgTime), out int time))
            Time = time;
        else
            Time = 60;
    }

    private static Dictionary<string, string> GetCommandArgsToValues(string[] args)
    {
        Dictionary<string, string> commandArgsToValues = new Dictionary<string, string>();
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == ArgTime)
                commandArgsToValues.TryAdd(args[i], args[++i]);
            if (args[i] == ArgToken)
                commandArgsToValues.TryAdd(args[i], args[++i]);
        }
        return commandArgsToValues;
    }
}
