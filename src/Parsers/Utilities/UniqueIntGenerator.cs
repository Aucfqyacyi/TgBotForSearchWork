namespace Parsers.Utilities;

public static class UniqueIntGenerator
{
    private static int _id = 0;

    public static int Generate()
    {
        Interlocked.Increment(ref _id);
        return _id;
    }
}
