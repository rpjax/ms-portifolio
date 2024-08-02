using Aidan.Core.Linq;
using MongoDB.Driver.Linq;
using System.Collections;
using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis;

public class WebqlQueryable : IQueryable<object>
{
    public IQueryable Source { get; }
    public Type ElementType { get; }
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }

    public WebqlQueryable(IQueryable source)
    {
        Source = source;
        ElementType = source.ElementType;
        Expression = source.Expression;
        Provider = source.Provider;
    }

    public IEnumerator GetEnumerator()
    {
        return Source.GetEnumerator();
    }

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
        return (IEnumerator<object>)Source.GetEnumerator();
    }

}

public class WebqlAsyncQueryable : IAsyncQueryable<object>
{
    public IAsyncQueryable Source { get; }
    public Type ElementType { get; }
    public Expression Expression { get; }
    public IQueryProvider Provider { get; }

    public WebqlAsyncQueryable(IAsyncQueryable source)
    {
        Source = source;
        ElementType = source.ElementType;
        Expression = source.Expression;
        Provider = source.Provider;
    }

    public IEnumerator<object> GetEnumerator()
    {
        return (IEnumerator<object>)Source.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return Source.GetEnumerator();
    }

    public async Task<double> AverageAsync(Expression<Func<object, double>> selector)
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "AverageAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, new object[] { selector })!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return (double)result!;
    }

    public async Task<int> CountAsync()
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "CountAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, new object[] { })!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return (int)result!;
    }

    public IAsyncQueryable<TResult> CreateQuery<TResult>(IQueryable<TResult> source)
    {
        var methodInfo = typeof(IAsyncQueryable<>)
            .MakeGenericType(ElementType)
            .GetMethod("CreateQuery")!
            .MakeGenericMethod(typeof(TResult));

        return (IAsyncQueryable<TResult>)methodInfo.Invoke(Source, new object[] { source })!;
    }

    public async Task<object> FirstAsync()
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "FirstAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, null)!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return result!;
    }

    public async Task<object?> FirstOrDefaultAsync()
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "FirstOrDefaultAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, null)!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return result;
    }

    public async Task<long> LongCountAsync()
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "LongCountAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, new object[] { })!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return (long)result!;
    }

    public async Task<object> MaxAsync()
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "MaxAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, null)!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return result!;
    }

    public async Task<object> MinAsync()
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "MinAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, null)!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return result!;
    }

    public async Task<object> SingleAsync()
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "SingleAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, null)!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return result!;
    }

    public async Task<object?> SingleOrDefaultAsync()
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "SingleOrDefaultAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, null)!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return result;
    }

    public async Task<decimal> SumAsync(Expression<Func<object, decimal>> selector)
    {
        var methodInfo = typeof(AsyncQueryableExtensions).GetMethods()
            .Where(x => x.Name == "SumAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, new object[] { selector })!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return (decimal)result!;
    }

    public async Task<object[]> ToArrayAsync()
    {
        var methodInfo = typeof(IAsyncQueryable<>).MakeGenericType(ElementType)
            .GetMethods()
            .Where(x => x.Name == "ToArrayAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, null)!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return (object[])result!;
    }

    public async Task<List<object>> ToListAsync()
    {
        var methodInfo = typeof(IAsyncQueryable<>).MakeGenericType(ElementType)
            .GetMethods()
            .Where(x => x.Name == "ToListAsync")
            .First();

        var task = (Task)methodInfo.Invoke(Source, null)!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        return (List<object>)result!;
    }

}
