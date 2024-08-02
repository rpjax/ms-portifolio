using Microsoft.EntityFrameworkCore;
using Aidan.Core.Linq;
using System.Collections;
using System.Linq.Expressions;

namespace Aidan.EntityFramework.Linq;

/// <summary>
/// Represents an asynchronous queryable wrapper for Entity Framework Core.
/// </summary>
/// <typeparam name="T">The type of the elements in the query.</typeparam>
public class EFCoreAsyncQueryable<T> : IAsyncQueryable<T> 
{
    /// <summary>
    /// Gets the type of the elements in the query.
    /// </summary>
    public Type ElementType => Source.ElementType;

    /// <summary>
    /// Gets the expression representing the query.
    /// </summary>
    public Expression Expression => Source.Expression;

    /// <summary>
    /// Gets the query provider.
    /// </summary>
    public IQueryProvider Provider => Source.Provider;

    private IQueryable<T> Source { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreAsyncQueryable{T}"/> class.
    /// </summary>
    /// <param name="source"> The source query. </param>
    public EFCoreAsyncQueryable(IQueryable<T> source)
    {
        Source = source;
    }

    /// <summary>
    /// Computes the average of the selected values asynchronously.
    /// </summary>
    /// <param name="selector">The expression used to select the values.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the average of the selected values.</returns>
    public Task<double> AverageAsync(Expression<Func<T, double>> selector)
    {
        return Source.AverageAsync(selector);
    }

    /// <summary>
    /// Returns the number of elements in the query asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements in the query.</returns>
    public Task<int> CountAsync()
    {
        return Source.CountAsync();
    }

    /// <summary>
    /// Creates a new instance of the <see cref="EFCoreAsyncQueryable{TResult}"/> class using the specified source query.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the new query.</typeparam>
    /// <param name="source">The source query.</param>
    /// <returns>A new instance of the <see cref="EFCoreAsyncQueryable{TResult}"/> class.</returns>
    public IAsyncQueryable<TResult> CreateQuery<TResult>(IQueryable<TResult> source)
    {
        return new EFCoreAsyncQueryable<TResult>(source);
    }

    /// <summary>
    /// Returns the first element of the query asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element of the query.</returns>
    public Task<T> FirstAsync()
    {
        return Source.FirstAsync();
    }

    /// <summary>
    /// Returns the first element of the query, or a default value if the query is empty, asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element of the query, or a default value if the query is empty.</returns>
    public Task<T?> FirstOrDefaultAsync()
    {
        return Source.FirstOrDefaultAsync();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the query results asynchronously.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the query results asynchronously.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return Source.GetEnumerator();
    }

    /// <summary>
    /// Returns the number of elements in the query as a 64-bit integer asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements in the query as a 64-bit integer.</returns>
    public Task<long> LongCountAsync()
    {
        return Source.LongCountAsync();
    }

    /// <summary>
    /// Returns the maximum value of the query asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the maximum value of the query.</returns>
    public Task<T> MaxAsync()
    {
        return Source.MaxAsync();
    }

    /// <summary>
    /// Returns the minimum value of the query asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the minimum value of the query.</returns>
    public Task<T> MinAsync()
    {
        return Source.MinAsync();
    }

    /// <summary>
    /// Returns the only element of the query asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the only element of the query.</returns>
    public Task<T> SingleAsync()
    {
        return Source.SingleAsync();
    }

    /// <summary>
    /// Returns the only element of the query, or a default value if the query is empty, asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the only element of the query, or a default value if the query is empty.</returns>
    public Task<T?> SingleOrDefaultAsync()
    {
        return Source.SingleOrDefaultAsync();
    }

    /// <summary>
    /// Computes the sum of the selected values asynchronously.
    /// </summary>
    /// <param name="selector">The expression used to select the values.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the sum of the selected values.</returns>
    public Task<decimal> SumAsync(Expression<Func<T, decimal>> selector)
    {
        return Source.SumAsync(selector);
    }

    /// <summary>
    /// Returns an array that contains the query results asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an array that contains the query results.</returns>
    public Task<T[]> ToArrayAsync()
    {
        return Source.ToArrayAsync();
    }

    /// <summary>
    /// Returns a list that contains the query results asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list that contains the query results.</returns>
    public Task<List<T>> ToListAsync()
    {
        return Source.ToListAsync();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Source.GetEnumerator();
    }
}
