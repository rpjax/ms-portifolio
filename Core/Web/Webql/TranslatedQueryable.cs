using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Represents a translated queryable object that encapsulates a WebQL query. <br/>
/// This class provides the capability to interact with query results as both IQueryable and IEnumerable, <br/>
/// supporting operations like enumeration, conversion to array, and count operations.
/// </summary>
public class TranslatedQueryable : IQueryable
{
    /// <summary>
    /// Gets the input type of the query.
    /// </summary>
    public Type InputType { get; }

    /// <summary>
    /// Gets the output type of the query, which can be a projection or a transformed type.
    /// </summary>
    public Type OutputType { get; }

    /// <summary>
    /// Gets the underlying query body, typically a WebQL query structure or expression tree.
    /// </summary>
    public object Body { get; }

    /// <inheritdoc/>
    public Type ElementType => AsQueryable().ElementType;

    /// <inheritdoc/>
    public Expression Expression => AsQueryable().Expression;

    /// <inheritdoc/>
    public IQueryProvider Provider => AsQueryable().Provider;

    private IQueryable? QueryableConversion { get; set; }
    private IEnumerable? EnumerableConversion { get; set; }

    /// <summary>
    /// Constructs a new instance of TranslatedQueryable with specified input and output types and a query body.
    /// </summary>
    /// <param name="inputType">The type of input data for the query.</param>
    /// <param name="outputType">The type of output data from the query.</param>
    /// <param name="body">The query body.</param>
    public TranslatedQueryable(Type inputType, Type outputType, object body)
    {
        InputType = inputType;
        OutputType = outputType;
        Body = body;
    }

    /// <summary>
    /// Constructs a new instance of TranslatedQueryable based on an existing instance.
    /// </summary>
    /// <param name="queryable">The existing TranslatedQueryable instance to clone.</param>
    public TranslatedQueryable(TranslatedQueryable queryable)
    {
        InputType = queryable.InputType;
        OutputType = queryable.OutputType;
        Body = queryable.Body;
    }

    /// <inheritdoc/>
    public IEnumerator GetEnumerator()
    {
        return AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Converts the queryable object to an IQueryable.
    /// </summary>
    /// <returns>An IQueryable representation of the query.</returns>
    public IQueryable AsQueryable()
    {
        if (QueryableConversion != null)
        {
            return QueryableConversion;
        }

        var method = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "AsQueryable" && !m.IsGenericMethod);

        return QueryableConversion = (IQueryable)method.Invoke(null, new[] { AsEnumerable() })!;
    }

    /// <summary>
    /// Converts the queryable object to an IEnumerable.
    /// </summary>
    /// <returns>An IEnumerable representation of the query.</returns>
    public IEnumerable AsEnumerable()
    {
        if (EnumerableConversion != null)
        {
            return EnumerableConversion;
        }

        var method = typeof(Enumerable)
            .GetMethod("AsEnumerable")!
            .MakeGenericMethod(OutputType);

        return (EnumerableConversion = (IEnumerable)method.Invoke(null, new object[] { Body })!);
    }

    /// <summary>
    /// Converts the query results to an array.
    /// </summary>
    /// <returns>An array containing the query results.</returns>
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

    /// <summary>
    /// Counts the number of elements in the query results.
    /// </summary>
    /// <returns>The number of elements.</returns>
    public int Count()
    {
        var method = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "Count" &&
                        m.GetParameters().Length == 1 &&
                        m.GetParameters()[0].ParameterType.IsGenericType &&
                        m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
            .MakeGenericMethod(OutputType);

        return (int)method.Invoke(null, new object[] { AsQueryable() })!;
    }

    /// <summary>
    /// Counts the number of elements in the query results for large datasets.
    /// </summary>
    /// <returns>The number of elements as a long.</returns>
    public long LongCount()
    {
        var method = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "LongCount" &&
                        m.GetParameters().Length == 1 &&
                        m.GetParameters()[0].ParameterType.IsGenericType &&
                        m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
            .MakeGenericMethod(OutputType);

        return (long)method.Invoke(null, new object[] { AsQueryable() })!;
    }

}

