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

    /// <summary>
    /// Asynchronously creates a new entity.
    /// </summary>
    /// <param name="entry">The entity to be created.</param>
    /// <returns>The ID of the created entity.</returns>
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

    /// <summary>
    /// Asynchronously creates multiple new entities.
    /// </summary>
    /// <param name="entries">The entities to be created.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
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

    /// <summary>
    /// Tries to asynchronously retrieve an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The retrieved entity or default if not found.</returns>
    public virtual async Task<T?> TryGetAsync(string id)
    {
        this.RunIdFormatValidation(id);

        var query = this.WhereIdEqualsQuery(id);
        var queryResult = await QueryAsync(query);

        if (queryResult.First == null)
        {
            return default;
        }

        return queryResult.First;
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The retrieved entity.</returns>
    /// <exception cref="AppException">Thrown when no entity matches the provided ID.</exception>
    public virtual async Task<T> GetAsync(string id)
    {
        var data = await TryGetAsync(id);

        if (data == null)
        {
            throw new AppException($"No entity found with the given ID: \"{id}\".", ExceptionCode.InvalidInput);
        }

        return data;
    }

    /// <summary>
    /// Asynchronously queries entities based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <returns>The results of the query.</returns>
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

    /// <summary>
    /// Asynchronously updates an entity.
    /// </summary>
    /// <param name="overrider">The entity with updated values.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
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

    /// <summary>
    /// Asynchronously deletes entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to determine which entities to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public virtual Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return DataAccessObject.DeleteAsync(this.Visit<T, Func<T, bool>>(predicate));
    }

    /// <summary>
    /// Asynchronously deletes all entities. Requires confirmation.
    /// </summary>
    /// <param name="confirm">If set to <c>true</c>, all entities will be deleted.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public virtual Task DeleteAllAsync(bool confirm = false)
    {
        if (confirm)
        {
            return DataAccessObject.DeleteAllAsync();
        }

        return Task.CompletedTask;
    }

    /// <summary>
    /// Asynchronously deletes an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
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

    /// <summary>
    /// Validates the format of the given ID.
    /// </summary>
    /// <param name="id">The ID to be validated.</param>
    /// <returns><c>true</c> if the ID format is valid; otherwise, <c>false</c>.</returns>
    public bool ValidateIdFormat(string id)
    {
        return DataAccessObject.ValidateIdFormat(id);
    }

    /// <summary>
    /// Asynchronously validates if the given ID exists in the data source.
    /// </summary>
    /// <param name="id">The ID to be validated.</param>
    /// <returns><c>true</c> if the ID exists; otherwise, <c>false</c>.</returns>
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

    /// <summary>
    /// Asynchronously counts entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to determine which entities to count.</param>
    /// <returns>The number of entities that match the predicate.</returns>
    public Task<long> CountAsync(Expression<Func<T, bool>> predicate)
    {
        return DataAccessObject.CountAsync(this.Visit<T, Func<T, bool>>(predicate));
    }

    /// <summary>
    /// Asynchronously counts all entities.
    /// </summary>
    /// <returns>The total number of entities.</returns>
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
    /// Maps the <see cref="IQueryableModel.GetId"/> method to its implementation.
    /// This should look like: <br/>
    /// MemberExpression => ParameterExpression
    /// </summary>
    /// <returns>The member expression mapped to the parameter expression.</returns>
    protected abstract MemberExpression CreateIdSelectorExpression(ParameterExpression parameter);

    /// <summary>
    /// Tries to convert the stringified version of the ID into it's implementation Type. 
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected abstract object? TryParseId(string id);

    /// <summary>
    /// Converts the stringified version of the ID into its implementation Type, throwing an exception if the conversion is unsuccessful.
    /// </summary>
    /// <param name="id">The string representation of the ID.</param>
    /// <returns>The parsed ID value.</returns>
    /// <exception cref="AppException">Thrown if the ID value cannot be parsed.</exception>
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

    /// <summary>
    /// Executes operations before the entity instance is created.
    /// By default, it sets the `CreatedAt` and `LastModifiedAt` properties.
    /// </summary>
    /// <param name="instance">The entity instance to be processed.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task BeforeCreateAsync(T instance)
    {
        instance.CreatedAt = TimeProvider.UtcNow();
        instance.LastModifiedAt = TimeProvider.UtcNow();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes operations after the entity instance has been created.
    /// </summary>
    /// <param name="instance">The created entity instance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task AfterCreateAsync(T instance)
    {
        return Task.CompletedTask;
    }

    //*
    // ON UPDATE CALLBACKS.
    //*

    /// <summary>
    /// Executes operations before the entity instance is updated.
    /// By default, it updates the `LastModifiedAt` property and maintains the values of `IsSoftDeleted` and `CreatedAt`.
    /// </summary>
    /// <param name="original">The original entity instance before the update.</param>
    /// <param name="overrider">The entity instance that contains the updates.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected virtual Task BeforeUpdateAsync(T original, T overrider)
    {
        overrider.IsSoftDeleted = overrider.IsSoftDeleted;
        overrider.CreatedAt = original.CreatedAt;
        overrider.LastModifiedAt = TimeProvider.UtcNow();
        return Task.CompletedTask;
    }

    /// <summary>
    /// Executes operations after the entity instance has been updated.
    /// </summary>
    /// <param name="original">The original entity instance before the update.</param>
    /// <param name="overrider">The updated entity instance.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
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
    /// <summary>
    /// Signals the LINQ provider to replace this flag with an <see cref="Expression"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="flagName"></param>
    /// <param name="data"></param>
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
