using MongoDB.Driver.Linq;
using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Represents a translated queryable object that encapsulates a WebQL query. <br/>
/// This class provides the capability to interact with query results as both IQueryable and IEnumerable, <br/>
/// supporting operations like enumeration, conversion to array, and count operations.
/// </summary>
public class TranslatedQueryable : IQueryable<object>
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

    /// <summary>
    /// Converts the queryable object to an <see cref="IEnumerable"/>. This method enables the conversion of the queryable object into an enumerable collection, <br/>
    /// facilitating operations like enumeration and iteration that are standard to collections in .NET.
    /// </summary>
    /// <returns>An <see cref="IEnumerable"/> representation of the query, allowing for enumeration and iteration over the query results.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the method invocation or conversion fails.</exception>
    public virtual IEnumerable AsEnumerable()
    {
        var method = typeof(Enumerable)
            .GetMethod("AsEnumerable")!
            .MakeGenericMethod(OutputType);

        return (IEnumerable)method.Invoke(null, new object[] { Body })!;
    }

    public virtual IEnumerable<T> AsEnumerable<T>()
    {
        return (IEnumerable<T>)AsEnumerable();
    }

    /// <summary>
    /// Converts the queryable object to an <see cref="IQueryable"/> with the <see cref="OutputType"/> as the generic type parameter. <br/>
    /// This method enables the seamless integration of the queryable object with standard LINQ operations.
    /// </summary>
    /// <returns>An <see cref="IQueryable"/> representation of the query, enabling standard LINQ query operations.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the method invocation or conversion fails.</exception>
    public virtual IQueryable AsQueryable()
    {
        var method = typeof(Queryable)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "AsQueryable" && m.IsGenericMethod)
            .MakeGenericMethod(OutputType);

        return (IQueryable)method.Invoke(null, new[] { Body })!;
    }

    public virtual IQueryable<T> AsQueryable<T>()
    {
        return (IQueryable<T>)AsEnumerable();
    }

    /// <inheritdoc/>
    public IEnumerator GetEnumerator()
    {
        return AsEnumerable().GetEnumerator();
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

    IEnumerator<object> IEnumerable<object>.GetEnumerator()
    {
        return (IEnumerator<object>)AsEnumerable().GetEnumerator();
    }

}
