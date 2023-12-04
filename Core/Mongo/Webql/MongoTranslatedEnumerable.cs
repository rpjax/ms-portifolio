using MongoDB.Driver;
using System.Collections;

namespace ModularSystem.Mongo.Webql;

/// <summary>
/// The representation of webql translation, with logic specific to the underlaying mongo library.
/// </summary>
public class MongoTranslatedEnumerable : IEnumerable
{
    public Type InputType { get; }
    public Type OutputType { get; }
    public object Enumerable { get; }

    private IEnumerable? EnumerableConversion { get; set; }

    public MongoTranslatedEnumerable(Type inputType, Type outputType, object enumerable)
    {
        InputType = inputType;
        OutputType = outputType;
        Enumerable = enumerable;
    }

    public IEnumerator GetEnumerator()
    {
        return AsEnumerable().GetEnumerator();
    }

    public IEnumerable AsEnumerable()
    {
        if (EnumerableConversion != null)
        {
            return EnumerableConversion;
        }

        var method = typeof(Enumerable)
            .GetMethod("AsEnumerable")!
            .MakeGenericMethod(OutputType);

        return EnumerableConversion = (method.Invoke(null, new[] { Enumerable }) as IEnumerable)!;
    }

    public object[] ToArray()
    {
        var method = typeof(Enumerable)
            .GetMethod("ToArray")!
            .MakeGenericMethod(OutputType);

        var enumerable = AsEnumerable();
        var result = method.Invoke(null, new[] { enumerable });

        if (result is not object[] array)
        {
            throw new Exception();
        }

        return array;
    }

    public Task<List<object>> ToListAsync()
    {
        var method = typeof(IAsyncCursorSourceExtensions)
            .GetMethods()
            .Where(x => x.Name == "ToListAsync")
            .First()
            .MakeGenericMethod(OutputType);

        var enumerable = Enumerable;
        var token = default(CancellationToken);
        Console.WriteLine(enumerable.GetType().FullName);
        var result = method.Invoke(null, new[] { enumerable, token });

        if (result is not Task task)
        {
            throw new Exception();
        }

        return (task as Task<List<object>>)!;
    }
}
