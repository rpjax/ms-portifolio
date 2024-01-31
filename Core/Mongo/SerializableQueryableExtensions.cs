using ModularSystem.Web;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace ModularSystem.Mongo.Linq;

public static class SerializableQueryableExtensions
{
    public static IMongoQueryable ToMongoQueryable<T>(
        this SerializableQueryable serializableQueryable,
        IQueryable<T> source)
    {
        return (IMongoQueryable) serializableQueryable.BuildQueryable(source);
    }

    public static async Task<object[]> ToArrayAsync<T>(
        this SerializableQueryable serializableQueryable,
        IQueryable<T> source)
    {
        using var cancellationTokenSource = new CancellationTokenSource();

        var queryable = ToMongoQueryable<T>(serializableQueryable, source);
        var outputType = queryable.GetType().GenericTypeArguments[0];
        var toListAsyncMethod = typeof(IAsyncCursorSourceExtensions)
            .GetMethod("ToListAsync")!
            .MakeGenericMethod(outputType);

        var task = (Task)toListAsyncMethod.Invoke(null, new object[] { queryable, cancellationTokenSource.Token })!;

        await task;
        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        var toArrayMethod = typeof(Enumerable)
            .GetMethod("ToArray")!
            .MakeGenericMethod(outputType);

        var toArrayResult = toArrayMethod.Invoke(null, new[] { result });

        if (toArrayResult is not object[] array)
        {
            throw new InvalidOperationException();
        }

        return array;

    }
}
