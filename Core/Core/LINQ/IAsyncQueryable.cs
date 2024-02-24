using System.Linq.Expressions;

namespace ModularSystem.Core.Linq;

/// <summary>
/// work in progress
/// </summary>
public interface IAsyncQueryProvider : IQueryProvider   
{
    Task<object> ExecuteAsync(Expression expression);
    Task<TResult> ExecuteAsync<TResult>(Expression expression);
}

/// <summary>
/// Represents an asynchronous version of the <see cref="IQueryable{T}"/> interface, <br/>
/// allowing LINQ queries to be executed asynchronously.
/// </summary>
/// <typeparam name="T">The type of elements in the queryable sequence.</typeparam>
public interface IAsyncQueryable<T> : IQueryable<T>
{
    /// <summary>
    /// Creates a new query from the provided <see cref="IQueryable{TResult}"/> source.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the returned query.</typeparam>
    /// <param name="source">The source query to convert.</param>
    /// <returns>An <see cref="IAsyncQueryable{TResult}"/> that represents the asynchronous query.</returns>
    IAsyncQueryable<TResult> CreateQuery<TResult>(IQueryable<TResult> source);

    /// <summary>
    /// Asynchronously generates a <see cref="List{T}"/> from the <see cref="IAsyncQueryable{T}"/>.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="List{T}"/> that contains elements from the input sequence.</returns>
    Task<List<T>> ToListAsync();

    /// <summary>
    /// Asynchronously generates an array of <typeparamref name="T"/>.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an array that contains elements from the input sequence.</returns>
    Task<T[]> ToArrayAsync();

    /// <summary>
    /// Asynchronously returns the first element of a sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element in the input sequence.</returns>
    Task<T> FirstAsync();

    /// <summary>
    /// Asynchronously returns the first element of a sequence, or a default value if the sequence contains no elements.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. <br/>
    /// The task result contains the first element in the input sequence or default value if the sequence contains no elements.</returns>
    Task<T> FirstOrDefaultAsync();

    /// <summary>
    /// Asynchronously returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the single element of the input sequence.</returns>
    Task<T> SingleAsync();

    /// <summary>
    /// Asynchronously returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the single element of the input sequence, or a default value if the sequence is empty.</returns>
    Task<T> SingleOrDefaultAsync();

    /// <summary>
    /// Asynchronously returns the number of elements in a sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements in the input sequence.</returns>
    Task<int> CountAsync();

    /// <summary>
    /// Asynchronously returns the number of elements in a sequence that satisfies a condition.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements in the input sequence that satisfies the condition.</returns>
    Task<long> LongCountAsync();

    /// <summary>
    /// Asynchronously computes the sum of the sequence of <see cref="Decimal"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the sum of the projected values.</returns>
    Task<decimal> SumAsync(Expression<Func<T, decimal>> selector);

    /// <summary>
    /// Asynchronously returns the minimum value in a sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the minimum value in the input sequence.</returns>
    Task<T> MinAsync();

    /// <summary>
    /// Asynchronously returns the maximum value in a sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the maximum value in the input sequence.</returns>
    Task<T> MaxAsync();

    /// <summary>
    /// Asynchronously computes the average of a sequence of <see cref="Double"/> values that are obtained by invoking a transform function on each element of the input sequence.
    /// </summary>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the average of the projected values.</returns>
    Task<double> AverageAsync(Expression<Func<T, double>> selector);
}

/// <summary>
/// Provides extension methods for <see cref="IAsyncQueryable{T}"/> to enable common query operations asynchronously.
/// </summary>
public static class AsyncQueryableExtensions
{
    /// <summary>
    /// Projects each element of a sequence into a new form asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
    /// <param name="source">An <see cref="IAsyncQueryable{T}"/> to filter.</param>
    /// <param name="selector">A projection function to apply to each element.</param>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements are the result of invoking the transform function on each element of source.</returns>
    public static IAsyncQueryable<TResult> Select<TSource, TResult>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TResult>> selector)
    {
        return source.CreateQuery(Queryable.Select(source, selector));
    }

    /// <summary>
    /// Filters a sequence of values based on a predicate asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">An <see cref="IAsyncQueryable{T}"/> to filter.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> that contains elements from the input sequence that satisfy the condition specified by predicate.</returns>
    public static IAsyncQueryable<TSource> Where<TSource>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, bool>> predicate)
    {
        return source.CreateQuery(Queryable.Where(source, predicate));
    }

    /// <summary>
    /// Projects each element of a sequence to an <see cref="IEnumerable{TResult}"/> and flattens the resulting sequences into one sequence asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TResult">The type of the elements of the sequence returned by selector.</typeparam>
    /// <param name="source">A sequence of values to project.</param>
    /// <param name="selector">A transform function to apply to each element.</param>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements are the result of invoking the one-to-many transform function on each element of the input sequence.</returns>
    public static IAsyncQueryable<TResult> SelectMany<TSource, TResult>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, IEnumerable<TResult>>> selector)
    {
        return source.CreateQuery(Queryable.SelectMany(source, selector));
    }

    /// <summary>
    /// Sorts the elements of a sequence in ascending order according to a key asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements are sorted according to a key.</returns>
    public static IAsyncQueryable<TSource> OrderBy<TSource, TKey>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
    {
        return source.CreateQuery(Queryable.OrderBy(source, keySelector));
    }

    /// <summary>
    /// Sorts the elements of a sequence in descending order according to a key asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">A sequence of values to order.</param>
    /// <param name="keySelector">A function to extract a key from an element.</param>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> whose elements are sorted in descending order according to a key.</returns>
    public static IAsyncQueryable<TSource> OrderByDescending<TSource, TKey>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
    {
        return source.CreateQuery(Queryable.OrderByDescending(source, keySelector));
    }

    /// <summary>
    /// Returns a specified number of contiguous elements from the start of a sequence asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="count">The number of elements to return.</param>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> that contains the specified number of elements from the start of the input sequence.</returns>
    public static IAsyncQueryable<TSource> Take<TSource>(this IAsyncQueryable<TSource> source, int count)
    {
        return source.CreateQuery(Queryable.Take(source, count));
    }

    /// <summary>
    /// Bypasses a specified number of elements in a sequence and then returns the remaining elements asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The sequence to return elements from.</param>
    /// <param name="count">The number of elements to skip before returning the remaining elements.</param>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> that contains elements that occur after the specified index in the input sequence.</returns>
    public static IAsyncQueryable<TSource> Skip<TSource>(this IAsyncQueryable<TSource> source, int count)
    {
        return source.CreateQuery(Queryable.Skip(source, count));
    }

    /// <summary>
    /// Returns distinct elements from a sequence by using the default equality comparer to compare values asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <param name="source">The sequence to remove duplicate elements from.</param>
    /// <returns>An <see cref="IAsyncQueryable{T}"/> that contains distinct elements from the source sequence.</returns>
    public static IAsyncQueryable<TSource> Distinct<TSource>(this IAsyncQueryable<TSource> source)
    {
        return source.CreateQuery(Queryable.Distinct(source));
    }

    /// <summary>
    /// Groups the elements of a sequence according to a specified key selector function asynchronously.
    /// </summary>
    /// <typeparam name="TSource">The type of the elements of source.</typeparam>
    /// <typeparam name="TKey">The type of the key returned by keySelector.</typeparam>
    /// <param name="source">The sequence whose elements to group.</param>
    /// <param name="keySelector">A function to extract the key for each element.</param>
    /// <returns>An <see cref="IAsyncQueryable{IGrouping{TKey, TSource}}"/> where each <see cref="IGrouping{TKey, TElement}"/> object contains a sequence of objects and a key.</returns>
    public static IAsyncQueryable<IGrouping<TKey, TSource>> GroupBy<TSource, TKey>(this IAsyncQueryable<TSource> source, Expression<Func<TSource, TKey>> keySelector)
    {
        return source.CreateQuery(Queryable.GroupBy(source, keySelector));
    }

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys and groups the results asynchronously.
    /// </summary>
    /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
    /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
    /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="outer">The first sequence to join.</param>
    /// <param name="inner">The sequence to join to the first sequence.</param>
    /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
    /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
    /// <param name="resultSelector">A function to create a result element from an element from the first sequence and a collection of matching elements from the second sequence.</param>
    /// <returns>An <see cref="IAsyncQueryable{TResult}"/> that contains elements of type TResult that are obtained by performing a grouped join on two sequences.</returns>
    public static IAsyncQueryable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(this IAsyncQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, IEnumerable<TInner>, TResult>> resultSelector)
    {
        return outer.CreateQuery(Queryable.GroupJoin(outer, inner, outerKeySelector, innerKeySelector, resultSelector));
    }

    /// <summary>
    /// Correlates the elements of two sequences based on matching keys asynchronously.
    /// </summary>
    /// <typeparam name="TOuter">The type of the elements of the first sequence.</typeparam>
    /// <typeparam name="TInner">The type of the elements of the second sequence.</typeparam>
    /// <typeparam name="TKey">The type of the keys returned by the key selector functions.</typeparam>
    /// <typeparam name="TResult">The type of the result elements.</typeparam>
    /// <param name="outer">The first sequence to join.</param>
    /// <param name="inner">The sequence to join to the first sequence.</param>
    /// <param name="outerKeySelector">A function to extract the join key from each element of the first sequence.</param>
    /// <param name="innerKeySelector">A function to extract the join key from each element of the second sequence.</param>
    /// <param name="resultSelector">A function to create a result element from two matching elements.</param>
    /// <returns>An <see cref="IAsyncQueryable{TResult}"/> that contains elements of type TResult that are obtained by performing an inner join on two sequences.</returns>
    public static IAsyncQueryable<TResult> Join<TOuter, TInner, TKey, TResult>(this IAsyncQueryable<TOuter> outer, IEnumerable<TInner> inner, Expression<Func<TOuter, TKey>> outerKeySelector, Expression<Func<TInner, TKey>> innerKeySelector, Expression<Func<TOuter, TInner, TResult>> resultSelector)
    {
        return outer.CreateQuery(Queryable.Join(outer, inner, outerKeySelector, innerKeySelector, resultSelector));
    }
}
