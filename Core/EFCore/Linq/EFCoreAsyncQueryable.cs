using Microsoft.EntityFrameworkCore;
using ModularSystem.Core.Linq;
using System.Collections;
using System.Linq.Expressions;

namespace ModularSystem.EntityFramework.Linq;

public class EFCoreAsyncQueryable<T> : IAsyncQueryable<T> 
{
    public Type ElementType => Source.ElementType;

    public Expression Expression => Source.Expression;

    public IQueryProvider Provider => Source.Provider;

    public EFCoreAsyncQueryable(IQueryable<T> source)
    {
        Source = source;
    }

    private IQueryable<T> Source { get; }

    public Task<double> AverageAsync(Expression<Func<T, double>> selector)
    {
        return Source.AverageAsync(selector);
    }

    public Task<int> CountAsync()
    {
        return Source.CountAsync();
    }

    public IAsyncQueryable<TResult> CreateQuery<TResult>(IQueryable<TResult> source) 
    {
        return new EFCoreAsyncQueryable<TResult>(source);
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
        return SingleOrDefaultAsync();
    }

    public Task<decimal> SumAsync(Expression<Func<T, decimal>> selector)
    {
        return Source.SumAsync(selector);
    }

    public Task<T[]> ToArrayAsync()
    {
        return Source.ToArrayAsync();
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
