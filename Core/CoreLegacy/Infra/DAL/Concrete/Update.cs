using ModularSystem.Core.Expressions;
using ModularSystem.Web;
using ModularSystem.Web.Expressions;
using System.Linq.Expressions;
using System.Text.Json;

namespace ModularSystem.Core;

/// <summary>
/// Represents an update operation for a given type, providing filter criteria and modifications.
/// </summary>
/// <typeparam name="T">The type of the entity being updated.</typeparam>
public class Update<T> : IUpdate<T>
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
    /// Initializes a new instance of the <see cref="Update{T}"/> class. 
    /// </summary>
    /// <param name="filter">The filter expression to determine which entities should be updated.</param>
    /// <param name="modifications">The list of modification expressions to be applied to the entities.</param>
    public Update(Expression? filter, Expression[] modifications)
    {
        Filter = filter;
        Modifications = modifications;
    }

    /// <summary>
    /// Converts the update into a <see cref="SerializableUpdate"/> format using <see cref="QueryProtocol"/>.
    /// </summary>
    /// <returns>A serialized representation of the update.</returns>
    public SerializableUpdate ToSerializable()
    {
        return new()
        {
            Filter = QueryProtocol.ToSerializable(Filter),
            Modifications = Modifications
                .Transform(x => QueryProtocol.ToSerializable(x))
                .ToArray()
        };
    }
}

/// <summary>
/// Provides a factory for building and refining <see cref="Update{T}"/> objects for entities of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// This factory is designed to be used in a fluent manner. 
/// The update creation and refinement methods return the factory itself, allowing for chaining of modifications.
/// </remarks>
public class UpdateBuilder<T> : IBuilder<Update<T>>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateBuilder{T}"/> class.
    /// </summary>
    /// <param name="conflictStrategy">The conflict strategy for handling conflicts when setting modifications.</param>
    public UpdateBuilder(ConflictStrategyType conflictStrategy = ConflictStrategyType.Throw)
    {
        ConflictStrategy = conflictStrategy;
        Filter = null;
        Modifications = new();
    }

    /// <summary>
    /// Creates an <see cref="UpdateBuilder{T}"/> instance from an existing <see cref="IUpdate{T}"/> object.
    /// </summary>
    /// <param name="update">The existing update object.</param>
    /// <param name="conflictStrategy">The conflict strategy for handling conflicts when setting modifications.</param>
    /// <returns>An <see cref="UpdateBuilder{T}"/> instance.</returns>
    public static UpdateBuilder<T> FromUpdate(IUpdate<T> update, ConflictStrategyType conflictStrategy = ConflictStrategyType.Throw)
    {
        return new UpdateBuilder<T>(conflictStrategy)
            .SetFilter(update.Filter)
            .SetModifications(update.Modifications);
    }

    /// <summary>
    /// Builds and returns the constructed update.
    /// </summary>
    /// <returns>The constructed update.</returns>
    public Update<T> Build()
    {
        return new Update<T>(Filter, Modifications.ToArray());
    }

    /// <summary>
    /// Sets the filter expression for the update.
    /// </summary>
    /// <param name="expression">The filter expression to determine which entities should be updated.</param>
    /// <returns>The current instance of the <see cref="UpdateBuilder{T}"/>.</returns>
    public UpdateBuilder<T> SetFilter(Expression<Func<T, bool>>? expression)
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
    /// <returns>The current instance of the <see cref="UpdateBuilder{T}"/>.</returns>
    public UpdateBuilder<T> SetModification<TField>(Expression<Func<T, TField>> selector, TField value)
    {
        var analyser = new SelectorExpressionAnalyzer<T, TField>(selector).Execute();

        var valueExpr = Expression.Constant(value, typeof(TField));

        var modification = new UpdateSetExpression(
            fieldName: analyser.GetFieldName(),
            type: analyser.GetFieldType(),
            selector: selector,
            value: valueExpr
        );

        var updatesForSelectedField = Modifications.Cast<UpdateSetExpression>()
            .Where(x => x.FieldName == modification.FieldName)
            .ToArray();

        if (updatesForSelectedField.IsEmpty())
        {
            Modifications.Add(modification);
            return this;
        }

        if (ConflictStrategy == ConflictStrategyType.Throw)
        {
            throw new InvalidOperationException($"A modification for the field '{modification.FieldName}' already exists. To override or skip this conflict, adjust the ConflictStrategy.");
        }
        if (ConflictStrategy == ConflictStrategyType.Skip)
        {
            return this;
        }

        var oldModification = updatesForSelectedField.First();

        Modifications.Remove(oldModification);
        Modifications.Add(modification);
        return this;
    }

    internal UpdateBuilder<T> SetFilter(Expression? expression)
    {
        Filter = expression;
        return this;
    }

    internal UpdateBuilder<T> SetModifications(IEnumerable<Expression> modifications)
    {
        foreach (var modification in modifications)
        {
            Modifications.Add(modification);
        }

        return this;
    }
}

/// <summary>
/// Serves as an interpretative tool for extracting and analyzing the expressions contained within an <see cref="Update{T}"/> data structure. <br/>
/// This class provides a specific way to interpret the encapsulated expressions of the update, but its architecture allows for potential <br/>
/// alternative interpretations in future implementations.
/// </summary>
/// <typeparam name="T">The type of the entity being targeted by the update.</typeparam>
public class UpdateReader<T>
{
    private Expression? Filter { get; }
    private Expression[] Modifications { get; }
    private Configs Config { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateReader{T}"/> class using an interface representation of an update.
    /// </summary>
    /// <param name="update">The update data structure containing the raw expressions to be interpreted.</param>
    /// <param name="configs">Optional configuration settings influencing the interpretation process.</param>
    public UpdateReader(IUpdate<T> update, Configs? configs = null)
    {
        Filter = update.Filter;
        Modifications = update.Modifications;
        Config = configs ?? new Configs();
    }

    /// <summary>
    /// Interprets and retrieves the filter expression encapsulated within the update.
    /// </summary>
    /// <returns>The interpreted filter expression, if present; otherwise, null.</returns>
    public Expression<Func<T, bool>>? GetFilterExpression()
    {
        return Config.UseParameterUniformityVisitor
            ? VisitExpression(Filter as Expression<Func<T, bool>>)
            : Filter as Expression<Func<T, bool>>;
    }

    /// <summary>
    /// Attempts to extract all update set expressions from the update without throwing exceptions for non-matching types.
    /// </summary>
    /// <returns>An enumerable of update set expressions.</returns>
    public IEnumerable<UpdateSetExpression> TryGetUpdateSetExpressions()
    {
        foreach (var expression in Modifications)
        {
            if (expression is UpdateSetExpression cast)
            {
                yield return cast;
            }
        }
    }

    /// <summary>
    /// Extracts all update set expressions from the update, throwing an exception if any non-matching type is encountered.
    /// </summary>
    /// <returns>An enumerable of update set expressions.</returns>
    public IEnumerable<UpdateSetExpression> GetUpdateSetExpressions()
    {
        foreach (var expression in Modifications)
        {
            if (expression is UpdateSetExpression updateSetExpression)
            {
                yield return updateSetExpression;
                continue;
            }

            throw new InvalidOperationException("Encountered a non-matching type during extraction.");
        }
    }

    /// <summary>
    /// Constructs an expression visitor to ensure uniformity in parameter references across combined expressions.
    /// </summary>
    /// <returns>A newly created expression visitor.</returns>
    protected ExpressionVisitor CreateExpressionVisitor()
    {
        return new ParameterExpressionReferenceBinder();
    }

    /// <summary>
    /// Visits and potentially modifies an expression.
    /// </summary>
    /// <typeparam name="TResult">Type of expression to be visited.</typeparam>
    /// <param name="expression">Expression to visit.</param>
    /// <returns>Modified expression, if any; otherwise the original expression.</returns>
    protected TResult? VisitExpression<TResult>(TResult? expression) where TResult : Expression
    {
        if (expression == null)
        {
            return null;
        }

        return CreateExpressionVisitor()
            .Visit(expression)
            .TypeCast<TResult>();
    }

    /// <summary>
    /// Configuration settings influencing the interpretation process of the <see cref="UpdateReader{T}"/>.
    /// </summary>
    public class Configs
    {
        /// <summary>
        /// Gets or sets a value indicating whether the ParameterUniformityVisitor should be used during the interpretation process to ensure consistent parameter references.
        /// </summary>
        public bool UseParameterUniformityVisitor { get; set; } = true;
    }
}
