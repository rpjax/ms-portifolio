namespace ModularSystem.Core;

public static class IEnumerableExtensions
{
    /// <summary>
    /// Iterates over elements in the source enumerable that match the specified condition and performs the given action on them.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerable">The source enumerable.</param>
    /// <param name="selector">A predicate function to select which elements the action should be applied to.</param>
    /// <param name="action">The action to execute on the matched elements.</param>
    /// <remarks>This method operates in-memory and does not modify the original enumerable. It is also not intended to produce a modified result for further LINQ queries.</remarks>
    public static void ForEachWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> selector, Action<T> action)
    {
        foreach (var item in enumerable.Where(selector))
        {
            action(item);
        }
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
