using ModularSystem.Webql.Synthesis;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Collections;
using System.Reflection;

namespace ModularSystem.Mongo.Webql;

public class MongoGeneratorOptions : TranslatorOptions
{
    public MongoGeneratorOptions()
    {
        TakeProvider = GetTakeProvider();
    }



    private MethodInfo GetTakeProvider()
    {
        throw new NotImplementedException();
        //MongoQueryable.Take
    }
}

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
    public MongoTranslatedQueryable(Type inputType, Type outputType, object body) : base(inputType, outputType, body)
    {
    }

    /// <summary>
    /// Initializes a new instance of the MongoTranslatedQueryable class based on an existing TranslatedQueryable instance.
    /// </summary>
    /// <param name="queryable">The existing TranslatedQueryable instance to base this instance on.</param>
    public MongoTranslatedQueryable(TranslatedQueryable queryable) : base(queryable)
    {
    }

    public override IQueryable AsQueryable()
    {
        var method = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "AsQueryable" && !m.IsGenericMethod);

        return (IMongoQueryable)method.Invoke(null, new[] { Body })!;
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
        var foo = AsQueryable();
        var token = default(CancellationToken);
        var task = (Task)method.Invoke(null, new object[] { AsQueryable(), token })!;

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
