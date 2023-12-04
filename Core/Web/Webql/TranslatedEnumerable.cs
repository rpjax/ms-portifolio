using System.Collections;

namespace ModularSystem.Webql;

public class TranslatedEnumerable : IEnumerable
{
    public Type InputType { get; }
    public Type OutputType { get; }
    public object Enumerable { get; }

    private IEnumerable? EnumerableConversion { get; set; }

    public TranslatedEnumerable(Type inputType, Type outputType, object enumerable)
    {
        InputType = inputType;
        OutputType = outputType;
        Enumerable = enumerable;
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

    public IEnumerator GetEnumerator()
    {
        return AsEnumerable().GetEnumerator();
    }
}

