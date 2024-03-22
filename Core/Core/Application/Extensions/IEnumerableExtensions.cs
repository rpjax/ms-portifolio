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

    public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        return enumerable.Where(x => !predicate.Invoke(x));
    }

    public static bool IsEmpty<T>(this IEnumerable<T> enumerable)
    {
        if (enumerable is Array array)
        {
            return array.Length == 0;
        }
        if (enumerable is ICollection<T> collection)
        {
            return collection.Count == 0;
        }

        return !enumerable.Any();
    }

    public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.IsEmpty();
    }

    /// <summary>
    /// Transforms each element in the source <see cref="IEnumerable{TOriginal}"/> using the provided converter function.
    /// </summary>
    /// <typeparam name="TOriginal">The type of the elements in the source collection.</typeparam>
    /// <typeparam name="TTransformed">The type of the elements in the resulting collection.</typeparam>
    /// <param name="source">The source collection to be transformed.</param>
    /// <param name="converter">A function that converts each element of type <typeparamref name="TOriginal"/> to type <typeparamref name="TTransformed"/>.</param>
    /// <returns>An <see cref="IEnumerable{TTransformed}"/> that contains each element of the source collection, transformed using the provided converter function.</returns>
    public static IEnumerable<TTransformed> Transform<TOriginal, TTransformed>(this IEnumerable<TOriginal> source, Func<TOriginal, TTransformed> converter)
    {
        foreach (var item in source)
        {
            yield return converter.Invoke(item);
        }
    }

    public static bool DoesNotContain<T>(
        this IEnumerable<T> enumerable,
        Func<T, bool> predicate
    )
    {
        return !enumerable.Any(x => predicate.Invoke(x));
    }

    public static bool DoesNotContain<T>(
        this IEnumerable<T> enumerable,
        T value
    )
    {
        return !enumerable.Any(x => x?.Equals(x) == true);
    }

}
