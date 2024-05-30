using ModularSystem.Core.Expressions;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Represents a builder for creating complex ordering expressions in queries.
/// </summary>
/// <typeparam name="T">The type of the entity being queried.</typeparam>
public class ComplexOrderingWriter<T> : IFactory<ComplexOrderingExpression>
{
    private List<OrderingExpression> Expressions { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexOrderingWriter{T}"/> class.
    /// </summary>
    /// <param name="expression">The existing complex ordering expression, if any.</param>
    public ComplexOrderingWriter(Expression? expression = null)
    {
        Expressions = new List<OrderingExpression>();

        if (expression != null && expression is ComplexOrderingExpression orderingExpression)
        {
            foreach (var item in orderingExpression.Expressions.OfType<OrderingExpression>())
            {
                Expressions.Add(item);
            }
        }
    }

    /// <summary>
    /// Creates a new instance of the complex ordering expression based on the added ordering expressions.
    /// </summary>
    /// <returns>The complex ordering expression.</returns>
    public ComplexOrderingExpression Create()
    {
        return new ComplexOrderingExpression(typeof(T), Expressions);
    }

    /// <summary>
    /// Clears all added ordering expressions from the builder.
    /// </summary>
    /// <returns>The current instance of the builder.</returns>
    public ComplexOrderingWriter<T> Clear()
    {
        Expressions.Clear();
        return this;
    }

    /// <summary>
    /// Adds an ordering expression to the complex ordering.
    /// </summary>
    /// <param name="fieldType">The type of the field used for ordering.</param>
    /// <param name="fieldSelector">The expression that selects the field from the data source for ordering.</param>
    /// <param name="direction">The ordering direction (ascending or descending).</param>
    /// <returns>The current instance of the builder.</returns>
    public ComplexOrderingWriter<T> AddOrdering(Type fieldType, Expression fieldSelector, OrderingDirection direction)
    {
        Expressions.Add(new OrderingExpression(fieldType, fieldSelector, direction));
        return this;
    }

    /// <summary>
    /// Adds an ordering expression to the complex ordering.
    /// </summary>
    /// <typeparam name="TField">The type of the field or property by which to sort.</typeparam>
    /// <param name="fieldSelector">The ordering expression to add.</param>
    /// <param name="direction">The ordering direction (ascending or descending).</param>
    /// <returns>The current instance of the builder.</returns>
    public ComplexOrderingWriter<T> AddOrdering<TField>(Expression<Func<T, TField>> fieldSelector, OrderingDirection direction)
    {
        Expressions.Add(new OrderingExpression(typeof(TField), fieldSelector, direction));
        return this;
    }

    /// <summary>
    /// Adds a complex ordering expression to the complex ordering.
    /// </summary>
    /// <param name="orderingExpression">The complex ordering expression to add.</param>
    /// <returns>The current instance of the builder.</returns>
    public ComplexOrderingWriter<T> AddOrdering(ComplexOrderingExpression orderingExpression)
    {
        Expressions.AddRange(orderingExpression.Expressions.OfType<OrderingExpression>());
        return this;
    }
}

/// Represents a reader for complex ordering expressions, extracting information about the entity type and ordering expressions.
/// </summary>
/// <typeparam name="T">The type of entity associated with the ordering expressions.</typeparam>
public class ComplexOrderingReader<T>
{
    /// <summary>
    /// Gets the complex ordering expression associated with the reader.
    /// </summary>
    private ComplexOrderingExpression? OrderingExpression { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComplexOrderingReader{T}"/> class.
    /// </summary>
    /// <param name="expression">The expression to extract complex ordering information from.</param>
    public ComplexOrderingReader(Expression? expression)
    {
        if (expression == null)
        {

        }
        if (expression is ComplexOrderingExpression orderingExpression)
        {
            OrderingExpression = orderingExpression;
        }
        else
        {
            throw new ArgumentException("Invalid argument type. Expected an expression of type ComplexOrderingExpression.", nameof(expression));
        }
    }

    /// <summary>
    /// Gets an enumerable of ordering expressions extracted from the complex ordering expression.
    /// </summary>
    /// <returns>An enumerable of ordering expressions.</returns>
    public IEnumerable<OrderingExpression> GetOrderingExpressions()
    {
        if (OrderingExpression == null)
        {
            yield break;
        }

        foreach (var item in OrderingExpression.Expressions)
        {
            if (item is OrderingExpression orderingExpression)
            {
                yield return orderingExpression;
            }
        }
    }

}
