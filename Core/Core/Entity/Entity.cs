using ModularSystem.Core.Expressions;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Abstract base class for entities, providing shared CRUD operations, serialization, and expression handling.
/// </summary>
/// <typeparam name="T">The type of the entity being operated on, which must implement IQueryableModel.</typeparam>
public abstract class Entity<T> : IEntity<T> where T : IQueryableModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the ID should be validated before deletion.
    /// </summary>
    public bool ValidateIdBeforeDeletion { get; set; }

    /// <summary>
    /// Gets the data access object associated with the entity.
    /// </summary>
    public abstract IDataAccessObject<T> DataAccessObject { get; }

    /// <summary>
    /// Gets the validator used for the entity. If no validator is provided, the entity won't be validated.
    /// </summary>
    public abstract IValidator<T>? Validator { get; init; }

    /// <summary>
    /// Gets the validator used for updating the entity. If no validator is provided, updates to the entity won't be validated.
    /// </summary>
    public abstract IValidator<T>? UpdateValidator { get; init; }

    /// <summary>
    /// Gets the validator used for querying the entity. If no validator is provided, queries won't be validated.
    /// </summary>
    public abstract IValidator<IQuery<T>>? QueryValidator { get; init; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{T}"/> class.
    /// </summary>
    protected Entity()
    {

    }

    /// <summary>
    /// Releases unmanaged resources and disposes of the managed resources used by the <see cref="DataAccessObject"/>.
    /// </summary>
    public virtual void Dispose()
    {
        DataAccessObject?.Dispose();
    }

    /// <summary>
    /// Factory method to create an expression visitor for the entity.
    /// </summary>
    /// <returns>A new instance of <see cref="IVisitor{Expression}"/> tailored for this entity.</returns>
    public virtual IVisitor<Expression> CreateExpressionVisitor()
    {
        return new EntityExpressionVisitor<T>()
        {
            CreateIdSelectorFunction = CreateIdSelectorExpression,
            ParseIdFunction = ParseId
        };
    }

    //*
    // CREATE.
    //*
    public virtual async Task<string> CreateAsync(T entry)
    {
        if (Validator != null)
        {
            var error = await Validator.ValidateAsync(entry);

            if (error != null)
            {
                throw error;
            }
        }

        await BeforeCreateAsync(entry);
        await DataAccessObject.InsertAsync(entry);
        await AfterCreateAsync(entry);
        return entry.GetId();
    }

    public virtual async Task CreateAsync(IEnumerable<T> entries)
    {
        var valdiationTasks = new List<Task<Exception?>>(entries.Count());

        if (Validator != null)
        {
            foreach (var entry in entries)
            {
                valdiationTasks.Add(Validator.ValidateAsync(entry));
            }

            await Task.WhenAll(valdiationTasks);
        }

        var validationExceptions = valdiationTasks
            .Where(x => x.Result != null)
            .Select(x => x.Result)
            .ToArray();

        if (validationExceptions != null && validationExceptions.IsNotEmpty())
        {
            var e = validationExceptions.First();

            if (e != null)
            {
                throw e;
            }
        }

        //*
        // Bulk BeforeCreate calls.
        //*

        var beforeCreateTasks = new List<Task>(entries.Count());

        foreach (var entry in entries)
        {
            beforeCreateTasks.Add(BeforeCreateAsync(entry));
        }

        await Task.WhenAll(beforeCreateTasks);

        //*
        //  DataAccessObject create call. 
        //*

        await DataAccessObject.InsertAsync(entries);

        //*
        // Bulk AfterCreate calls.
        //*

        var afterCreateTasks = new List<Task>(entries.Count());

        foreach (var entry in entries)
        {
            afterCreateTasks.Add(AfterCreateAsync(entry));
        }

        await Task.WhenAll(afterCreateTasks);
    }

    //*
    // READ.
    //*
    public virtual async Task<T?> TryGetAsync(string id)
    {
        await this.RunIdValidationAsync(id);

        var query = this.WhereIdEqualsQuery(id);
        var queryResult = await QueryAsync(query);

        if (queryResult.IsEmpty)
        {
            return default;
        }

        return queryResult.First!;
    }

    public virtual async Task<T> GetAsync(string id)
    {
        var data = await TryGetAsync(id);

        if (data == null)
        {
            throw new InvalidOperationException();
        }

        return data;
    }

    public virtual async Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        if (QueryValidator != null)
        {
            var error = await QueryValidator.ValidateAsync(query);

            if (error != null)
            {
                throw error;
            }
        }

        return await DataAccessObject.QueryAsync(this.Visit(query));
    }

    //*
    // UPDATE.
    //*
    public virtual async Task UpdateAsync(T overrider)
    {
        var original = await GetAsync(overrider.GetId());

        if (UpdateValidator != null)
        {
            var error = await UpdateValidator.ValidateAsync(overrider);

            if (error != null)
            {
                throw error;
            }
        }

        await BeforeUpdateAsync(original, overrider);
        await DataAccessObject.UpdateAsync(overrider);
        await AfterUpdateAsync(original, overrider);
    }

    //*
    // DELETE.
    //*
    public virtual Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return DataAccessObject.DeleteAsync(this.Visit<T, Func<T, bool>>(predicate));
    }

    public virtual Task DeleteAllAsync(bool confirm = false)
    {
        if (confirm)
        {
            return DataAccessObject.DeleteAllAsync();
        }

        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(string id)
    {
        if (ValidateIdBeforeDeletion)
        {
            await this.RunIdValidationAsync(id);
        }

        await DataAccessObject.DeleteAsync(this.WhereIdEquals(id));
    }

    //*
    // ID VALIDATIONS.
    //*
    public bool ValidateIdFormat(string id)
    {
        return DataAccessObject.ValidateIdFormat(id);
    }

    public virtual async Task<bool> ValidateIdAsync(string id)
    {
        if (DataAccessObject.ValidateIdFormat(id))
        {
            long count = await DataAccessObject.CountAsync(this.WhereIdEquals(id));
            return count > 0;
        }

        return false;
    }

    //*
    // OTHER STUFF.
    //*
    public Task<long> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return DataAccessObject.CountAsync(this.Visit<T, Func<T, bool>>(predicate));
    }

    public virtual Task<long> CountAllAsync()
    {
        return DataAccessObject.CountAllAsync();
    }

    //*
    // TREE VISITORS.
    //*

    /// <summary>
    /// Visits the specified expression and replaces interfaces with their concrete implementations.<br/>
    /// This method delegates the visitation to a custom expression visitor created by the <see cref="CreateExpressionVisitor"/> method.
    /// </summary>
    /// <param name="expression">The expression to be visited.</param>
    /// <returns>The visited expression with interfaces replaced by their concrete implementations if applicable, or null if the provided expression is null.</returns>
    [return: NotNullIfNotNull("expression")]
    public virtual Expression? Visit(Expression? expression)
    {
        return CreateExpressionVisitor().Visit(expression);
    }

    //*
    // EXPRESSION MAPPINGS
    //*

    /// <summary>
    /// Maps the <see cref="IQueryableModel.GetId"/> method to its implementation.<br/>
    /// This should look like this:<br/>
    /// <code>              
    /// MemberExpression => ParameterExpression    
    /// </code>
    /// </summary>
    /// <returns></returns>
    protected abstract MemberExpression CreateIdSelectorExpression(ParameterExpression parameter);

    /// <summary>
    /// Tries to convert the stringified version of the ID into it's implementation Type. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected abstract object? TryParseId(string id);

    protected virtual object ParseId(string id)
    {
        var parsedValue = TryParseId(id);

        if (parsedValue == null)
        {
            throw new AppException($"The ID value \"{id}\" does could not be parsed.", ExceptionCode.InvalidInput);
        }

        return parsedValue;
    }

    //*
    // ON CREATE CALLBACKS.
    //*
    protected virtual Task BeforeCreateAsync(T instance)
    {
        instance.CreatedAt = TimeProvider.UtcNow();
        instance.LastModifiedAt = TimeProvider.UtcNow();
        return Task.CompletedTask;
    }

    protected virtual Task AfterCreateAsync(T instance)
    {
        return Task.CompletedTask;
    }

    //*
    // ON UPDATE CALLBACKS.
    //*
    protected virtual Task BeforeUpdateAsync(T original, T overrider)
    {
        overrider.IsSoftDeleted = overrider.IsSoftDeleted;
        overrider.CreatedAt = original.CreatedAt;
        overrider.LastModifiedAt = TimeProvider.UtcNow();
        return Task.CompletedTask;
    }

    protected virtual Task AfterUpdateAsync(T original, T overrider)
    {
        return Task.CompletedTask;
    }

}

//*
// EXPRESSION REPLACEMENTS/FLAGS.
// NOTE: those methods are not suposed to called, but instead, replaced by the expression visitor.
//*

/// <summary>
/// Static helper class providing LINQ-related methods for the Entity class.<br></br>
/// Those methods are not supposed to be called, but instead, replaced by the expression visitor.
/// </summary>
/// <remarks>These methods are placeholders for LINQ operations and should be replaced by an expression visitor.</remarks>

public static class EntityLinq
{
    //public const string IdEqualsFlag = "ID_EQUALS";

    /// <summary>
    /// Signals the LINQ provider to replace this flag with an <see cref="Expression"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flagName"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static T ReplacementFlag<T>(string flagName, object? data = null)
    {
        throw new Exception();
    }

    /// <summary>
    /// Signals the LINQ provider to replace this flag with an <see cref="Expression"/>.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static bool IdEqualsFlag(string id)
    {
        throw new Exception();
    }
}
