using System.Linq.Expressions;

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
    public Expression? Filter { get; set; } = null;

    /// <summary>
    /// Gets or sets the list of modification expressions to be applied to the entities.
    /// </summary>
    public List<Expression> Modifications { get; set; } = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Update{T}"/> class.
    /// Optionally, an existing update can be provided to initialize the new instance.
    /// </summary>
    /// <param name="update">An existing update to initialize the new instance, if any.</param>
    public Update(IUpdate<T>? update = null)
    {
        if (update == null) return;
        Filter = update.Filter;
        Modifications = update.Modifications;
    }
}

/// <summary>
/// Provides a builder pattern for constructing update operations for a given type.
/// </summary>
/// <typeparam name="T">The type of the entity being updated.</typeparam>
public class UpdateWriter<T> : IFactory<IUpdate<T>>
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

    /// <summary>
    /// Gets or sets the strategy to use when a conflict is detected.
    /// </summary>
    public ConflictStrategyType ConflictStrategy { get; set; }

    /// <summary>
    /// Gets the update being constructed.
    /// </summary>
    private Update<T> Update { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateWriter{T}"/> class.
    /// </summary>
    /// <param name="update">An existing update to initialize the writer, if provided.</param>
    /// <param name="conflictStrategy">The strategy to use when a conflict is detected during modification setting. Defaults to <see cref="ConflictStrategyType.Throw"/>.</param>
    public UpdateWriter(IUpdate<T>? update = null, ConflictStrategyType conflictStrategy = ConflictStrategyType.Throw)
    {
        Update = new();
        if (update == null) return;
        Update.Filter = update.Filter;
        Update.Modifications = update.Modifications;
    }

    /// <summary>
    /// Creates and returns the constructed update.
    /// </summary>
    /// <returns>The constructed update.</returns>
    public IUpdate<T> Create()
    {
        return Update;
    }

    /// <summary>
    /// Sets the filter expression for the update.
    /// </summary>
    /// <param name="expression">The filter expression.</param>
    /// <returns>The current instance of the writer.</returns>
    public UpdateWriter<T> SetFilter(Expression<Func<T, bool>>? expression)
    {
        Update.Filter = expression;
        return this;
    }

    /// <summary>
    /// Sets a modification for a specific field in the update.
    /// </summary>
    /// <typeparam name="TField">The type of the field being modified.</typeparam>
    /// <param name="selector">The selector expression for the field.</param>
    /// <param name="value">The new value for the field.</param>
    /// <returns>The current instance of the writer.</returns>
    public UpdateWriter<T> SetModification<TField>(Expression<Func<T, TField>> selector, TField value)
    {
        var analyser = new SelectorExpressionAnalyzer<T, TField>(selector)
            .Execute();
        var modification = 
            new UpdateSetExpression(analyser.GetFieldName(), analyser.GetFieldType(), value);
        var reader = new UpdateReader<T>(Update);

        var updatesForSelectedField = reader
            .TryGetUpdateSetExpressions()
            .Where(x => x.FieldName == modification.FieldName)
            .ToArray();

        if(updatesForSelectedField.IsEmpty())
        {
            Update.Modifications.Add(modification);
            return this;
        }
        
        if(ConflictStrategy == ConflictStrategyType.Throw)
        {
            throw new InvalidOperationException("");
        }
        if(ConflictStrategy == ConflictStrategyType.Skip)
        {
            return this;
        }

        var oldModification = updatesForSelectedField.First();

        Update.Modifications.Remove(oldModification);
        Update.Modifications.Add(modification);
        return this;
    }
    
}

public class UpdateReader<T>
{
    private Update<T> Update { get; }
    private Configs Config { get; }

    public UpdateReader(Update<T> update)
    {
        Update = update;
    }

    public UpdateReader(IUpdate<T> update)
    {
        Update = new(update);
    }

    public Expression<Func<T, bool>>? GetFilterExpression()
    {
        return VisitExpression(Update.Filter as Expression<Func<T, bool>>);
    }

    public IEnumerable<UpdateSetExpression> TryGetUpdateSetExpressions()
    {
        foreach (var expression in Update.Modifications)
        {
            var cast = expression as UpdateSetExpression;

            if (cast == null)
            {
                continue;
            }

            yield return cast;
        }
    }

    public IEnumerable<UpdateSetExpression> GetUpdateSetExpressions()
    {
        foreach (var expression in Update.Modifications)
        {
            var cast = expression as UpdateSetExpression;

            if(cast == null)
            {
                throw new InvalidOperationException();
            }

            yield return cast;
        }
    }

    /// <summary>
    /// Constructs an expression visitor to ensure uniformity in parameter references across combined expressions.
    /// </summary>
    /// <returns>A newly created expression visitor.</returns>
    protected ExpressionVisitor CreateExpressionVisitor()
    {
        return new ParameterExpressionUniformityVisitor();
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

        return CreateExpressionVisitor().Visit(expression).TypeCast<TResult>();
    }

    /// <summary>
    /// Configuration settings for the <see cref="UpdateReader{T}"/>.
    /// </summary>
    public class Configs
    {
        /// <summary>
        /// Gets or sets a value indicating whether to use the ParameterUniformityVisitor for ensuring consistent parameter references.
        /// </summary>
        public bool UseParameterUniformityVisitor { get; set; } = true;
    }
}