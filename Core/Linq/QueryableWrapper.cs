using System.Collections;
using System.Linq.Expressions;

namespace Aidan.Core.Linq;

/// <summary>
/// Provides a wrapper around an IQueryable source, allowing it to be treated as an <see cref="IQueryable{object}"/>.
/// </summary>
/// <remarks>
/// This class is useful for working with IQueryable sources where the element type is not known at compile time.<br/>
/// It provides a way to interact with these sources using a common object type.
/// </remarks>
public class QueryableWrapper : IQueryable<object>
{
    /// <summary>
    /// Gets the type of the element(s) that are stored in the IQueryable source.
    /// </summary>
    public Type ElementType { get; }

    /// <summary>
    /// Gets the expression tree that is associated with the instance of IQueryable.
    /// </summary>
    public Expression Expression { get; }

    /// <summary>
    /// Gets the query provider that is associated with this data source.
    /// </summary>
    public IQueryProvider Provider { get; }

    private IQueryable Source { get; }
    private object[]? Result { get; set; }

    /// <summary>
    /// Initializes a new instance of the QueryableWrapper class.
    /// </summary>
    /// <param name="source">The IQueryable source to be wrapped.</param>
    /// <exception cref="ArgumentException">Thrown if the source expression does not resolve to an IQueryable&lt;T&gt;.</exception>
    public QueryableWrapper(IQueryable source)
    {
        var expression = source.Expression;

        if (!expression.Type.IsGenericType)
        {
            throw new ArgumentException("The expression must resolve to an IQueryable<T>.");
        }

        ElementType = expression.Type.GenericTypeArguments[0];
        Expression = expression;
        Provider = source.Provider;
        Source = source;
    }

    /// <summary>
    /// Converts the source IQueryable to an IEnumerable of objects.
    /// </summary>
    /// <returns>An IEnumerable<object> representing the materialized results of the query.</returns>
    protected virtual IEnumerable<object> AsEnumerable()
    {
        return Materialize();
    }

    /// <summary>
    /// Returns an enumerator that iterates through the collection.
    /// </summary>
    /// <returns>An IEnumerator<object> that can be used to iterate through the collection.</returns>
    public IEnumerator<object> GetEnumerator()
    {
        return AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Returns an enumerator that iterates through a collection.
    /// </summary>
    /// <returns>An IEnumerator that can be used to iterate through the collection.</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return AsEnumerable().GetEnumerator();
    }

    /// <summary>
    /// Materializes the IQueryable source into an array of objects.
    /// </summary>
    /// <returns>An array of objects representing the materialized results of the query.</returns>
    /// <exception cref="InvalidOperationException">Thrown if the materialization fails and does not produce an object array.</exception>
    protected object[] Materialize()
    {
        if (Result != null)
        {
            return Result;
        }

        var method = typeof(Enumerable)
           .GetMethod("ToArray")!
           .MakeGenericMethod(ElementType);

        var result = method.Invoke(null, new[] { Source });

        if (result is not object[] array)
        {
            throw new InvalidOperationException("Failed to materialize IQueryable source into an object array.");
        }

        Result = array;
        return Result;
    }
}
