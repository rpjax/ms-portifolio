using ModularSystem.Core.Patterns;
using System.Linq.Expressions;

namespace ModularSystem.Core.Linq.Expressions;

/// <summary>
/// Represents an interface for update expressions.
/// </summary>
public interface IUpdateExpression
{
    /// <summary>
    /// Gets the filter expression used to filter the data to be updated.
    /// </summary>
    Expression? Filter { get; }

    /// <summary>
    /// Gets an array of expressions representing the modifications to be made to the data.
    /// </summary>
    Expression[] Modifications { get; }
}

/// <summary>
/// Represents an update expression for modifying entities.
/// </summary>
public class UpdateExpression : IUpdateExpression
{
    /// <summary>
    /// Gets or sets the filter expression to determine which entities should be updated.
    /// </summary>
    public Expression? Filter { get; }

    /// <summary>
    /// Gets or sets the list of modification expressions to be applied to the entities.
    /// </summary>
    public Expression[] Modifications { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateExpression"/> class.
    /// </summary>
    /// <param name="filter"></param>
    /// <param name="modifications"></param>
    public UpdateExpression(Expression? filter, Expression[] modifications)
    {
        Filter = filter;
        Modifications = modifications;
    }

}

/// <summary>
/// Provides a factory for building and refining <see cref="UpdateExpression"/> objects for entities of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// This factory is designed to be used in a fluent manner. 
/// The update creation and refinement methods return the factory itself, allowing for chaining of modifications.
/// </remarks>
public class UpdateExpressionBuilder<T> : IBuilder<UpdateExpression>
{
    /// <summary>
    /// Defines strategies for handling conflicts when setting modifications.
    /// </summary>
    public enum ConflictStrategyType
    {
        /// <summary>
        /// Throws an exception when a conflict is detected.
        /// </summary>
        Throw,

        /// <summary>
        /// Skips the modification when a conflict is detected.
        /// </summary>
        Skip,

        /// <summary>
        /// Overrides the existing modification when a conflict is detected.
        /// </summary>
        Override
    }

    private ConflictStrategyType ConflictStrategy { get; set; }
    private Expression? Filter { get; set; }
    private List<Expression> Modifications { get; }
    private List<UpdateModificationExpression> UpdateModifications { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateExpressionBuilder{T}"/> class.
    /// </summary>
    /// <param name="conflictStrategy">The conflict strategy for handling conflicts when setting modifications.</param>
    public UpdateExpressionBuilder(ConflictStrategyType conflictStrategy = ConflictStrategyType.Throw)
    {
        ConflictStrategy = conflictStrategy;
        Filter = null;
        Modifications = new();
        UpdateModifications = new();
    }

    /// <summary>
    /// Creates an <see cref="UpdateExpressionBuilder{T}"/> instance from an existing <see cref="IUpdateExpression"/> object.
    /// </summary>
    /// <param name="expression">The existing update object.</param>
    /// <param name="conflictStrategy">The conflict strategy for handling conflicts when setting modifications.</param>
    /// <returns>An <see cref="UpdateExpressionBuilder{T}"/> instance.</returns>
    public static UpdateExpressionBuilder<T> FromExpression(
        IUpdateExpression expression, 
        ConflictStrategyType conflictStrategy = ConflictStrategyType.Throw)
    {
        return new UpdateExpressionBuilder<T>(conflictStrategy)
            .SetFilter(expression.Filter)
            .SetModifications(expression.Modifications);
    }

    /// <summary>
    /// Builds and returns the constructed update.
    /// </summary>
    /// <returns>The constructed update.</returns>
    public UpdateExpression Build()
    {
        return new UpdateExpression(Filter, Modifications.ToArray());
    }

    /// <summary>
    /// Sets the filter expression for the update.
    /// </summary>
    /// <param name="expression">The filter expression to determine which entities should be updated.</param>
    /// <returns>The current instance of the <see cref="UpdateExpressionBuilder{T}"/>.</returns>
    public UpdateExpressionBuilder<T> SetFilter(Expression<Func<T, bool>>? expression)
    {
        Filter = expression;
        return this;
    }

    /// <summary>
    /// Sets a modification for a specific field in the update.
    /// </summary>
    /// <typeparam name="TField">The type of the field being modified.</typeparam>
    /// <param name="selector">The selector expression for the field.</param>
    /// <param name="value">The new value for the field.</param>
    /// <returns>The current instance of the <see cref="UpdateExpressionBuilder{T}"/>.</returns>
    public UpdateExpressionBuilder<T> SetModification<TField>(Expression<Func<T, TField>> selector, TField value)
    {
        var modificationExpression = new UpdateModificationExpression(
            selector: selector.Body,
            value: Expression.Constant(value, typeof(TField))
        );

        var valueExpr = Expression.Constant(value, typeof(TField));

        throw new NotImplementedException();        
    }

    internal UpdateExpressionBuilder<T> SetFilter(Expression? expression)
    {
        Filter = expression;
        return this;
    }

    internal UpdateExpressionBuilder<T> SetModifications(IEnumerable<Expression> modifications)
    {
        foreach (var modification in modifications)
        {
            Modifications.Add(modification);
        }

        return this;
    }
}

/// <summary>
/// Serves as an interpretative tool for extracting and analyzing the expressions contained within an <see cref="UpdateExpression"/> data structure. <br/>
/// This class provides a specific way to interpret the encapsulated expressions of the update, but its architecture allows for potential <br/>
/// alternative interpretations in future implementations.
/// </summary>
/// <typeparam name="T">The type of the entity being targeted by the update.</typeparam>
public class UpdateExpressionReader<T>
{
    private Expression? Filter { get; }
    private Expression[] Modifications { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateExpressionReader{T}"/> class using an interface representation of an update.
    /// </summary>
    /// <param name="update">The update data structure containing the raw expressions to be interpreted.</param>
    public UpdateExpressionReader(IUpdateExpression update)
    {
        Filter = update.Filter;
        Modifications = update.Modifications;
    }

    /// <summary>
    /// Interprets and retrieves the filter expression encapsulated within the update.
    /// </summary>
    /// <returns>The interpreted filter expression, if present; otherwise, null.</returns>
    public Expression<Func<T, bool>>? GetFilterExpression()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Attempts to extract all update set expressions from the update without throwing exceptions for non-matching types.
    /// </summary>
    /// <returns>An enumerable of update set expressions.</returns>
    public IEnumerable<UpdateModificationExpression> TryGetModificationExpressions()
    {
        foreach (var expression in Modifications)
        {
            if (expression is UpdateModificationExpression cast)
            {
                yield return cast;
            }
        }
    }

    /// <summary>
    /// Extracts all update set expressions from the update, throwing an exception if any non-matching type is encountered.
    /// </summary>
    /// <returns>An enumerable of update set expressions.</returns>
    public IEnumerable<UpdateModificationExpression> GetModificationExpressions()
    {
        foreach (var expression in Modifications)
        {
            if (expression is UpdateModificationExpression updateSetExpression)
            {
                yield return updateSetExpression;
                continue;
            }

            throw new InvalidOperationException("Encountered a non-matching type during extraction.");
        }
    }

}
