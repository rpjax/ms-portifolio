using ModularSystem.Webql.Synthesis;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections;
using System.Reflection;

namespace ModularSystem.Mongo.Webql;

/// <summary>
/// Represents a WebQL translated queryable object with MongoDB specific logic. <br/>
/// This class extends the basic WebQL translated queryable functionalities
/// to support MongoDB's asynchronous operations.
/// </summary>
public class MongoTranslatedQueryable : TranslatedQueryable
{
    /// <summary>
    /// Initializes a new instance of the MongoTranslatedQueryable class with specified types and query body.
    /// </summary>
    /// <param name="inputType">The type of input data for the query.</param>
    /// <param name="outputType">The type of output data from the query.</param>
    /// <param name="body">The query body or expression tree.</param>
    public MongoTranslatedQueryable(Type inputType, Type outputType, object body)
        : base(inputType, outputType, body)
    {
    }

    /// <summary>
    /// Initializes a new instance of the MongoTranslatedQueryable class based on an existing TranslatedQueryable instance.
    /// </summary>
    /// <param name="queryable">The existing TranslatedQueryable instance to base this instance on.</param>
    public MongoTranslatedQueryable(TranslatedQueryable queryable) : base(queryable)
    {
    }

    /// <summary>
    /// Converts the queryable object to an <see cref="IMongoQueryable"/> using the <see cref="OutputType"/> as the generic type parameter. <br/>
    /// This method adapts the queryable object to be compatible with MongoDB's LINQ provider, allowing seamless integration and query operations against MongoDB collections.
    /// </summary>
    /// <returns>An <see cref="IMongoQueryable"/> representation of the query, enabling the use of MongoDB's LINQ capabilities for querying MongoDB collections.</returns>
    /// <exception cref="InvalidCastException">Thrown if the conversion to <see cref="IMongoQueryable"/> is not successful.</exception>
    public IMongoQueryable AsMongoQueryable()
    {
        return (IMongoQueryable)AsQueryable();
    }

    /// <summary>
    /// Asynchronously converts the query results to a list.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of objects representing the query results.</returns>
    public async Task<List<object>> ToListAsync()
    {
        var method = typeof(IAsyncCursorSourceExtensions)
            .GetMethods()
            .Where(x => x.Name == "ToListAsync")
            .First()
            .MakeGenericMethod(OutputType);

        var token = default(CancellationToken);
        var task = (Task)method.Invoke(null, new object[] { AsMongoQueryable(), token })!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;
        var result = resultProperty.GetValue(task);

        if (result == null)
        {
            throw new Exception();
        }

        return ((IEnumerable)result).Cast<object>().ToList();
    }

    /// <summary>
    /// Asynchronously converts the query results to an array.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains an array of the query results.</returns>
    public override async Task<object[]> ToArrayAsync()
    {
        return (await ToListAsync()).ToArray();
    }

    /// <summary>
    /// Asynchronously counts the number of elements in the query results.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements.</returns>
    public async Task<int> CountAsync()
    {
        var method = typeof(MongoQueryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "CountAsync")
            .Where(x =>
            {
                var methodParams = x.GetParameters();

                return
                    methodParams.Length == 2
                    && methodParams[0].ParameterType.IsGenericType
                    && methodParams[0].ParameterType.GetGenericTypeDefinition() == typeof(IMongoQueryable<>)
                    && methodParams[1].ParameterType == typeof(CancellationToken);
            })
            .First()
            .MakeGenericMethod(OutputType);

        var task = (Task)method.Invoke(null, new object[] { AsQueryable(), default(CancellationToken) })!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;

        return (int)resultProperty.GetValue(task)!;
    }

    /// <summary>
    /// Asynchronously counts the number of elements in the query results for large datasets.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result contains the number of elements as a long.</returns>
    public async Task<long> LongCountAsync()
    {
        var method = typeof(MongoQueryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .Where(m => m.Name == "LongCountAsync")
            .Where(x =>
            {
                var methodParams = x.GetParameters();

                return
                    methodParams.Length == 2
                    && methodParams[0].ParameterType.IsGenericType
                    && methodParams[0].ParameterType.GetGenericTypeDefinition() == typeof(IMongoQueryable<>)
                    && methodParams[1].ParameterType == typeof(CancellationToken);
            })
            .First()
            .MakeGenericMethod(OutputType);

        var task = (Task)method.Invoke(null, new object[] { AsQueryable(), default(CancellationToken) })!;

        await task;

        var resultProperty = task.GetType().GetProperty("Result")!;

        return (long)resultProperty.GetValue(task)!;
    }

}
