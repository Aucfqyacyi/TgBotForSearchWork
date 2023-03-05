namespace TgBotForSearchWorkApi.Extensions;

public static class IEnumerableExtension
{
    public static bool NotContains<TValue>(IEnumerable<TValue> values, TValue value)
    {
        return values.Contains(value) is false;
    }
}
