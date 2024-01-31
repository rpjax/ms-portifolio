using System.Threading;

namespace ModularSystem.Core.Linq;

/// <summary>
/// Defines an extended IEnumerable interface for a specific type, including additional query capabilities and asynchronous operations.
/// </summary>
/// <typeparam name="T">The type of the elements in the enumerable sequence.</typeparam>
public interface IExtendedEnumerable<T> : IEnumerable<T>
{
    /// <summary>
    /// Asynchronously creates an array from the IEnumerable sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and contains the array of queried elements.</returns>
    Task<T[]> ToArrayAsync();
}

/// <summary>
/// Defines an extended IQueryable interface that includes additional query capabilities.
/// </summary>
public interface IExtendedQueryable : IQueryable
{

}

/// <summary>
/// Defines an extended IQueryable interface for a specific type, including additional query capabilities and asynchronous operations.
/// </summary>
/// <typeparam name="T">The type of the elements in the queryable sequence.</typeparam>
public interface IExtendedQueryable<T> : IExtendedQueryable, IQueryable<T>
{
    Task<T[]> ToArrayAsync();
}

public static class AsyncEnumerableExtensions
{
    public static async Task<T[]> ToArrayAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await using var enumerator = source.GetAsyncEnumerator();

        while(await enumerator.MoveNextAsync())
        {
            list.Add(enumerator.Current);
        }

        return list.ToArray();
    }

    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> source)
    {
        var list = new List<T>();
        await using var enumerator = source.GetAsyncEnumerator();

        while (await enumerator.MoveNextAsync())
        {
            list.Add(enumerator.Current);
        }

        return list;
    }
}
