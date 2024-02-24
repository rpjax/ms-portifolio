using ModularSystem.Core;
using ModularSystem.Core.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections;
using System.Linq.Expressions;

namespace ModularSystem.Mongo.Linq;

public class MongoAsyncQueryable<T> : IAsyncQueryable<T>
{
    public Type ElementType => Source.ElementType;

    public Expression Expression => Source.Expression;

    public IQueryProvider Provider => Source.Provider;

    public MongoAsyncQueryable(IMongoQueryable<T> source)
    {
        Source = source;
    }

    private IMongoQueryable<T> Source { get; }

    public IAsyncQueryable<TResult> CreateQuery<TResult>(IQueryable<TResult> source)
    {
        return new MongoAsyncQueryable<TResult>(source.TypeCast<IMongoQueryable<TResult>>());
    }

    public Task<double> AverageAsync(Expression<Func<T, double>> selector)
    {
        return Source.AverageAsync(selector);
    }

    public Task<int> CountAsync()
    {
        return Source.CountAsync();
    }

    public Task<T> FirstAsync()
    {
        return Source.FirstAsync();
    }

    public Task<T> FirstOrDefaultAsync()
    {
        return Source.FirstOrDefaultAsync();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return Source.GetEnumerator();
    }

    public Task<long> LongCountAsync()
    {
        return Source.LongCountAsync();
    }

    public Task<T> MaxAsync()
    {
        return Source.MaxAsync();
    }

    public Task<T> MinAsync()
    {
        return Source.MinAsync();
    }

    public Task<T> SingleAsync()
    {
        return Source.SingleAsync();
    }

    public Task<T> SingleOrDefaultAsync()
    {
        return Source.SingleOrDefaultAsync();
    }

    public Task<decimal> SumAsync(Expression<Func<T, decimal>> selector)
    {
        return Source.SumAsync(selector);
    }

    public async Task<T[]> ToArrayAsync()
    {
        using (var cursor = await Source.ToCursorAsync())
        {
            return (await cursor.ToListAsync()).ToArray();
        }
    }

    public Task<List<T>> ToListAsync()
    {
        return Source.ToListAsync();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Source.GetEnumerator();
    }
}
