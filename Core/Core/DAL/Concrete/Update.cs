using ModularSystem.Web;
using ModularSystem.Web.Expressions;
using MongoDB.Bson;
using System.Linq;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
    
    /// <summary>
    /// Converts the update into a <see cref="SerializableUpdate"/> format.
    /// </summary>
    /// <param name="serializer">An optional serializer to use for the serialization. If not provided, the default serializer will be used.</param>
    /// <returns>A serialized representation of the update.</returns>
    public SerializableUpdate ToSerializable(ExpressionSerializer? serializer = null)
    {
        return new()
        {
            Filter = QueryProtocol.ToJson(Filter, serializer),
            Modifications = Modifications
                .Transform(x => QueryProtocol.ToJson(x, serializer)).ToArray()
        };
    }
}

/// <summary>
/// Provides a factory for building and refining <see cref="Update{T}"/> objects for entities of type <typeparamref name="T"/>.
/// </summary>
/// <remarks>
/// This factory is designed to be used in a fluent manner. <br/>
/// The update creation and refinement methods return the factory itself, allowing for chaining of modifications.
/// </remarks>
public class UpdateWriter<T> : IFactory<Update<T>>
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

        if (update != null)
        {
            Update.Filter = update.Filter;
            Update.Modifications = update.Modifications;
        }
    }

    /// <summary>
    /// Creates and returns the constructed update.
    /// </summary>
    /// <returns>The constructed update.</returns>
    public Update<T> Create()
    {
        return Update;
    }

    /// <summary>
    /// Produces a serializable representation of the <see cref="Update{T}"/> object as constructed by this factory.
    /// </summary>
    /// <returns>The serialized representation of the constructed query object.</returns>
    public SerializableUpdate CreateSerializable()
    {
        return Update.ToSerializable();
    }

    /// <summary>
    /// Produces a <see cref="string"/> representation of the <see cref="Update{T}"/> object as constructed by this factory.
    /// </summary>
    /// <returns>The serialized string representation of the constructed query object.</returns>
    public string CreateSerialized(ISerializer? serializer = null)
    {
        serializer ??= new ExprToUtf8Serializer();
        return serializer.Serialize(CreateSerializable());
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
            throw new InvalidOperationException($"A modification for the field '{modification.FieldName}' already exists. To override or skip this conflict, adjust the ConflictStrategy.");
        }
        if (ConflictStrategy == ConflictStrategyType.Skip)
        {
            return this;
        }

        var oldModification = updatesForSelectedField.First();

        Update.Modifications.Remove(oldModification);
        Update.Modifications.Add(modification);
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
    private readonly Update<T> Update;
    private readonly Configs Config;

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateReader{T}"/> class, offering an interpretative perspective over the provided update data structure.
    /// </summary>
    /// <param name="update">The update data structure containing the raw expressions to be interpreted.</param>
    /// <param name="configs">Optional configuration settings influencing the interpretation process.</param>
    public UpdateReader(Update<T> update, Configs? configs = null)
    {
        Update = update ?? throw new ArgumentNullException(nameof(update));
        Config = configs ?? new Configs();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateReader{T}"/> class using an interface representation of an update.
    /// </summary>
    /// <param name="update">The update data structure containing the raw expressions to be interpreted.</param>
    /// <param name="configs">Optional configuration settings influencing the interpretation process.</param>
    public UpdateReader(IUpdate<T> update, Configs? configs = null)
        : this(new Update<T>(update), configs)
    {
    }

    /// <summary>
    /// Interprets and retrieves the filter expression encapsulated within the update.
    /// </summary>
    /// <returns>The interpreted filter expression, if present; otherwise, null.</returns>
    public Expression<Func<T, bool>>? GetFilterExpression()
    {
        return Config.UseParameterUniformityVisitor
            ? VisitExpression(Update.Filter as Expression<Func<T, bool>>)
            : Update.Filter as Expression<Func<T, bool>>;
    }

    /// <summary>
    /// Attempts to extract all update set expressions from the update without throwing exceptions for non-matching types.
    /// </summary>
    /// <returns>An enumerable of update set expressions.</returns>
    public IEnumerable<UpdateSetExpression> TryGetUpdateSetExpressions()
    {
        foreach (var expression in Update.Modifications)
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
        foreach (var expression in Update.Modifications)
        {
            if (expression is UpdateSetExpression cast)
            {
                yield return cast;
            }
            else
            {
                throw new InvalidOperationException("Encountered a non-matching type during extraction.");
            }
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
