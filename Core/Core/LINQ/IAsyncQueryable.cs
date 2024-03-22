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
/// Represents an asynchronous version of the <see cref="IQueryable"/> interface.
/// </summary>
public interface IAsyncQueryable : IQueryable
{
    ///// <summary>
    ///// Creates a new query from the provided <see cref="IQueryable{TResult}"/> source.
    ///// </summary>
    ///// <param name="source">The source query to convert.</param>
    ///// <returns>An <see cref="IAsyncQueryable{TResult}"/> that represents the asynchronous query.</returns>
    //IAsyncQueryable CreateQuery(IQueryable source);
}

/// <summary>
/// Represents an asynchronous version of the <see cref="IQueryable{T}"/> interface, <br/>
/// allowing LINQ queries to be executed asynchronously.
/// </summary>
/// <typeparam name="T">The type of elements in the queryable sequence.</typeparam>
public interface IAsyncQueryable<T> : IQueryable<T>, IAsyncQueryable
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
    Task<T?> FirstOrDefaultAsync();

    /// <summary>
    /// Asynchronously returns the only element of a sequence, and throws an exception if there is not exactly one element in the sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the single element of the input sequence.</returns>
    Task<T> SingleAsync();

    /// <summary>
    /// Asynchronously returns the only element of a sequence, or a default value if the sequence is empty; this method throws an exception if there is more than one element in the sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the single element of the input sequence, or a default value if the sequence is empty.</returns>
    Task<T?> SingleOrDefaultAsync();

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
