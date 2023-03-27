namespace TgBotForSearchWorkApi.Extensions;

public static class IEnumerableExtension
{
    public static bool NotContains<TValue>(IEnumerable<TValue> values, TValue value)
    {
        return values.Contains(value) is false;
    }

    public static bool Contains<TEntity>(this IEnumerable<TEntity> first, IEnumerable<TEntity> second)
    {
        bool contains = false;
        foreach (TEntity entity in second)
        {
            contains = first.Contains(entity);
        }
        return contains;
    }
}
