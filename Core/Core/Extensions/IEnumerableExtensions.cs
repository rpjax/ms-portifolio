namespace ModularSystem.Core;

public static class IEnumerableExtensions
{
    public static void Update<T>(this IEnumerable<T> enumerable, Func<T, bool> selector, Action<T> action)
    {
        enumerable.Where(selector).Select(x =>
        {
            action.Invoke(x);
            return x;
        });
    }

    public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T> list, Func<T, bool> selector)
    {
        return list.Where(x => !selector.Invoke(x));
    }

    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.Any();
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable)
    {
        return enumerable.Any();
    }

    public static T? GetOneWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        var query = enumerable.Where(predicate);
        return query.IsEmpty() ? default(T) : query.First();
    }
}
