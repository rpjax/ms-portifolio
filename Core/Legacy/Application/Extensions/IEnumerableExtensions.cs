using System.Runtime.CompilerServices;

namespace ModularSystem.Core.Extensions;

/// <summary>
/// A collection of extension methods for <see cref="IEnumerable{T}"/> objects.
/// </summary>
public static class IEnumerableExtensions
{
    /// <summary>
    /// Determines whether the specified enumerable is empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <returns><c>true</c> if the enumerable is empty; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    /// <summary>
    /// Determines whether the specified enumerable is not empty.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to check.</param>
    /// <returns><c>true</c> if the enumerable is not empty; otherwise, <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsNotEmpty<T>(this IEnumerable<T> enumerable)
    {
        return !enumerable.IsEmpty();
    }

    /// <summary>
    /// Transforms each element in the source enumerable using the provided converter function.
    /// </summary>
    /// <typeparam name="TOriginal">The type of the elements in the source collection.</typeparam>
    /// <typeparam name="TTransformed">The type of the elements in the resulting collection.</typeparam>
    /// <param name="source">The source collection to be transformed.</param>
    /// <param name="converter">A function that converts each element of type <typeparamref name="TOriginal"/> to type <typeparamref name="TTransformed"/>.</param>
    /// <returns>An enumerable that contains each element of the source collection, transformed using the provided converter function.</returns>
    public static IEnumerable<TTransformed> Transform<TOriginal, TTransformed>(this IEnumerable<TOriginal> source, Func<TOriginal, TTransformed> converter)
    {
        foreach (var item in source)
        {
            yield return converter.Invoke(item);
        }
    }

    /// <summary>
    /// Removes elements from the enumerable that satisfy the specified predicate.
    /// </summary>
    /// <typeparam name="T">The type of the elements in the enumerable.</typeparam>
    /// <param name="enumerable">The enumerable to remove elements from.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An enumerable that contains the elements from the original enumerable, excluding those that satisfy the predicate.</returns>
    public static IEnumerable<T> RemoveWhere<T>(this IEnumerable<T> enumerable, Func<T, bool> predicate)
    {
        return enumerable.Where(x => !predicate.Invoke(x));
    }
}
