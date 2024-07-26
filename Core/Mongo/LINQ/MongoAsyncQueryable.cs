using ModularSystem.Core;
using ModularSystem.Core.Extensions;
using ModularSystem.Core.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections;
using System.Linq.Expressions;

namespace ModularSystem.Mongo.Linq;

/// <summary>
/// Represents an asynchronous queryable collection in MongoDB.
/// </summary>
/// <typeparam name="T">The type of the elements in the collection.</typeparam>
public class MongoAsyncQueryable<T> : IAsyncQueryable<T>
{
    /// <summary>
    /// Gets the type of the elements in the collection.
    /// </summary>
    public Type ElementType => Source.ElementType;

    /// <summary>
    /// Gets the expression representing the query.
    /// </summary>
    public Expression Expression => Source.Expression;

    /// <summary>
    /// Gets the query provider for the collection.
    /// </summary>
    public IQueryProvider Provider => Source.Provider;

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoAsyncQueryable{T}"/> class.
    /// </summary>
    /// <param name="source">The source queryable collection.</param>
    public MongoAsyncQueryable(IMongoQueryable<T> source)
    {
        Source = source;
    }

    private IMongoQueryable<T> Source { get; }

    /// <summary>
    /// Creates a new queryable collection of a different type.
    /// </summary>
    /// <typeparam name="TResult">The type of the elements in the new collection.</typeparam>
    /// <param name="source">The source queryable collection.</param>
    /// <returns>A new queryable collection of the specified type.</returns>
    public IAsyncQueryable<TResult> CreateQuery<TResult>(IQueryable<TResult> source)
    {
        return new MongoAsyncQueryable<TResult>(source.TypeCast<IMongoQueryable<TResult>>());
    }

    /// <summary>
    /// Computes the average of the selected values in the collection asynchronously.
    /// </summary>
    /// <param name="selector">The expression specifying the values to average.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the average value.</returns>
    public Task<double> AverageAsync(Expression<Func<T, double>> selector)
    {
        return Source.AverageAsync(selector);
    }

    /// <summary>
    /// Counts the number of elements in the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements.</returns>
    public Task<int> CountAsync()
    {
        return Source.CountAsync();
    }

    /// <summary>
    /// Returns the first element of the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element.</returns>
    public Task<T> FirstAsync()
    {
        return Source.FirstAsync();
    }

    /// <summary>
    /// Returns the first element of the collection, or a default value if the collection is empty, asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the first element, or the default value if the collection is empty.</returns>
    public async Task<T?> FirstOrDefaultAsync()
    {
        var data = await Source.FirstOrDefaultAsync();

        if(data == null)
        {
            return default;
        }

        return data;
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An enumerator that can be used to iterate through the collection.</returns>
    public IEnumerator<T> GetEnumerator()
    {
        return Source.GetEnumerator();
    }

    /// <summary>
    /// Counts the number of elements in the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements.</returns>
    public Task<long> LongCountAsync()
    {
        return Source.LongCountAsync();
    }

    /// <summary>
    /// Returns the maximum value in the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the maximum value.</returns>
    public Task<T> MaxAsync()
    {
        return Source.MaxAsync();
    }

    /// <summary>
    /// Returns the minimum value in the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the minimum value.</returns>
    public Task<T> MinAsync()
    {
        return Source.MinAsync();
    }

    /// <summary>
    /// Returns the only element of the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the only element.</returns>
    public Task<T> SingleAsync()
    {
        return Source.SingleAsync();
    }

    /// <summary>
    /// Returns the only element of the collection, or a default value if the collection is empty, asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the only element, or the default value if the collection is empty.</returns>
    public async Task<T?> SingleOrDefaultAsync()
    {
        var data = await Source.SingleOrDefaultAsync();

        if(data == null)
        {
            return default;
        }

        return data;
    }

    /// <summary>
    /// Computes the sum of the selected values in the collection asynchronously.
    /// </summary>
    /// <param name="selector">The expression specifying the values to sum.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the sum of the values.</returns>
    public Task<decimal> SumAsync(Expression<Func<T, decimal>> selector)
    {
        return Source.SumAsync(selector);
    }

    /// <summary>
    /// Returns an array containing all the elements in the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an array of the elements.</returns>
    public async Task<T[]> ToArrayAsync()
    {
        using (var cursor = await Source.ToCursorAsync())
        {
            return (await cursor.ToListAsync()).ToArray();
        }
    }

    /// <summary>
    /// Returns a list containing all the elements in the collection asynchronously.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of the elements.</returns>
    public Task<List<T>> ToListAsync()
    {
        return Source.ToListAsync();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Source.GetEnumerator();
    }
}
