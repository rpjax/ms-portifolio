using ModularSystem.Core.Expressions;
using System.Collections.Concurrent;
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
    public IValidator<T>? Validator { get; init; }

    /// <summary>
    /// Gets the validator used for updating the entity. If no validator is provided, updates to the entity won't be validated.
    /// </summary>
    public IValidator<T>? UpdateValidator { get; init; }

    /// <summary>
    /// Gets the validator used for querying the entity. If no validator is provided, queries won't be validated.
    /// </summary>
    public IValidator<IQuery<T>>? QueryValidator { get; init; }

    /// <summary>
    /// Retrieves an instance of a middleware wrapper that encapsulates the hooks of the current entity.
    /// </summary>
    /// <returns>An instance representing the hooks of the entity.</returns>
    public EntityMiddleware<T> Hooks => new HooksWrapper(this);

    private ConcurrentBag<EntityMiddleware<T>> Middlewares { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Entity{T}"/> class.
    /// </summary>
    protected Entity()
    {
        Validator = null;
        UpdateValidator = null;
        QueryValidator = null;
        Middlewares = new();
        AddInternalMiddlewares();
    }

    /// <summary>
    /// Releases unmanaged resources and disposes of the managed resources used by the <see cref="DataAccessObject"/>.
    /// </summary>
    public virtual void Dispose()
    {
        DataAccessObject?.Dispose();
    }

    /// <summary>
    /// Adds a middleware to the list of middlewares for this entity.
    /// </summary>
    /// <param name="middleware">The middleware to add.</param>
    public void AddMiddleware(EntityMiddleware<T> middleware)
    {
        Middlewares.Add(middleware);
    }

    /// <summary>
    /// Adds a middleware of the specified type to the list of middlewares for this entity.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of middleware to add.</typeparam>
    public void AddMiddleware<TMiddleware>() where TMiddleware : EntityMiddleware<T>, new()
    {
        Middlewares.Add(new TMiddleware());
    }

    /// <summary>
    /// Clears all middlewares from the list.
    /// </summary>
    public void ClearMiddlewares()
    {
        Middlewares.Clear();
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
        await BeforeCreateAsync(entry);
        await DataAccessObject.InsertAsync(entry);
        await AfterCreateAsync(entry);

        return entry.GetId();
    }

    //*
    // READ.
    //*

    /// <summary>
    /// Asynchronously queries entities based on the provided query parameters.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    /// <returns>The results of the query.</returns>
    public virtual async Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        await BeforeQueryAsync(query);
        var queryResult = await DataAccessObject.QueryAsync(this.Visit(query));
        await AfterQueryAsync(queryResult);

        return queryResult;
    }

    //*
    // UPDATE.
    //*

    /// <summary>
    /// Asynchronously updates an entity.
    /// </summary>
    /// <param name="instance">The entity with updated values.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    public virtual async Task UpdateAsync(T instance)
    {
        var original = await this.GetAsync(instance.GetId());

        await BeforeUpdateAsync(original, instance);
        await DataAccessObject.UpdateAsync(instance);
        await AfterUpdateAsync(original, instance);
    }

    //*
    // DELETE.
    //*

    /// <summary>
    /// Asynchronously deletes entities that match the specified predicate.
    /// </summary>
    /// <param name="predicate">The predicate to determine which entities to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public virtual async Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        await BeforeDeleteAsync(predicate);
        await DataAccessObject.DeleteAsync(this.Visit<T, Func<T, bool>>(predicate));
        await AfterDeleteAsync(predicate);
    }

    /// <summary>
    /// Asynchronously deletes all entities. Requires confirmation.
    /// </summary>
    /// <param name="confirm">If set to <c>true</c>, all entities will be deleted.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public virtual async Task DeleteAllAsync(bool confirm = false)
    {
        if (!confirm)
        {
            return;
        }

        await BeforeDeleteAllAsync();
        await DataAccessObject.DeleteAllAsync();
        await AfterDeleteAllAsync();
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
    public async Task<long> CountAsync(Expression<Func<T, bool>> predicate)
    {
        await BeforeCountAsync(predicate);
        var count = await DataAccessObject.CountAsync(this.Visit<T, Func<T, bool>>(predicate));
        await AfterCountAsync(predicate);

        return count;
    }

    /// <summary>
    /// Asynchronously counts all entities.
    /// </summary>
    /// <returns>The total number of entities.</returns>
    public virtual async Task<long> CountAllAsync()
    {
        await BeforeCountAllAsync();
        var count = await DataAccessObject.CountAllAsync();
        await AfterCountAllAsync();

        return count;
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

    private void AddInternalMiddlewares()
    {
        AddMiddleware(new ValidationMiddleware<T>(this));
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
    // HOOKS
    //*

    //*
    // before validation hook
    //*

    /// <summary>
    /// Called before the validation of the entity.
    /// </summary>
    /// <param name="entity">The entity to be validated.</param>
    protected virtual async Task BeforeValidateAsync(T entity)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.BeforeValidateAsync(entity);
        }
    }

    //*
    // creation hooks
    //*

    /// <summary>
    /// Called before creating the entity.
    /// </summary>
    /// <param name="entity">The entity to be created.</param>
    protected virtual async Task BeforeCreateAsync(T entity)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.BeforeCreateAsync(entity);
        }
    }

    /// <summary>
    /// Called after creating the entity.
    /// </summary>
    /// <param name="entity">The recently created entity.</param>
    protected virtual async Task AfterCreateAsync(T entity)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.AfterCreateAsync(entity);
        }
    }

    //*
    // query hooks
    //*

    /// <summary>
    /// Called before querying the entity.
    /// </summary>
    /// <param name="query">The query to be executed.</param>
    protected virtual async Task BeforeQueryAsync(IQuery<T> query)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.BeforeQueryAsync(query);
        }
    }

    /// <summary>
    /// Called after querying the entity.
    /// </summary>
    /// <param name="queryResult">The result of the query.</param>
    protected virtual async Task AfterQueryAsync(IQueryResult<T> queryResult)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.AfterQueryAsync(queryResult);
        }
    }

    //*
    // update hooks
    //*

    /// <summary>
    /// Called before updating the entity.
    /// </summary>
    /// <param name="old">The current state of the entity.</param>
    /// <param name="new">The new state of the entity.</param>
    protected virtual async Task BeforeUpdateAsync(T old, T @new)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.BeforeUpdateAsync(old, @new);
        }
    }

    /// <summary>
    /// Called after updating the entity.
    /// </summary>
    /// <param name="old">The state of the entity before the update.</param>
    /// <param name="new">The updated state of the entity.</param>
    protected virtual async Task AfterUpdateAsync(T old, T @new)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.AfterUpdateAsync(old, @new);
        }
    }

    //*
    // deletion hooks
    //*

    /// <summary>
    /// Called before deleting the entity based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate used for the delete operation.</param>
    protected virtual async Task BeforeDeleteAsync(Expression<Func<T, bool>> predicate)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.BeforeDeleteAsync(predicate);
        }
    }

    /// <summary>
    /// Called after deleting the entity based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate used for the delete operation.</param>
    protected virtual async Task AfterDeleteAsync(Expression<Func<T, bool>> predicate)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.AfterDeleteAsync(predicate);
        }
    }

    //*
    // delete all hooks
    //*

    /// <summary>
    /// Called before deleting all entities.
    /// </summary>
    protected virtual async Task BeforeDeleteAllAsync()
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.BeforeDeleteAllAsync();
        }
    }

    /// <summary>
    /// Called after deleting all entities.
    /// </summary>
    protected virtual async Task AfterDeleteAllAsync()
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.AfterDeleteAllAsync();
        }
    }

    //*
    // count hooks
    //*

    /// <summary>
    /// Called before counting the entities based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate used for the count operation.</param>
    protected virtual async Task BeforeCountAsync(Expression<Func<T, bool>> predicate)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.BeforeCountAsync(predicate);
        }
    }

    /// <summary>
    /// Called after counting the entities based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate used for the count operation.</param>
    protected virtual async Task AfterCountAsync(Expression<Func<T, bool>> predicate)
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.AfterCountAsync(predicate);
        }
    }

    //*
    // count all hooks
    //*

    /// <summary>
    /// Called before counting all entities.
    /// </summary>
    protected virtual async Task BeforeCountAllAsync()
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.BeforeCountAllAsync();
        }
    }

    /// <summary>
    /// Called after counting all entities.
    /// </summary>
    protected virtual async Task AfterCountAllAsync()
    {
        foreach (var middleware in Middlewares)
        {
            await middleware.AfterCountAllAsync();
        }
    }

    internal class HooksWrapper : EntityMiddleware<T>
    {
        private Entity<T> Entity { get; }

        public HooksWrapper(Entity<T> entity)
        {
            Entity = entity;
        }

        public override Task BeforeValidateAsync(T entity)
        {
            return Entity.BeforeValidateAsync(entity);
        }

        public override Task BeforeCreateAsync(T entity)
        {
            return Entity.BeforeCreateAsync(entity);
        }

        public override Task AfterCreateAsync(T entity)
        {
            return Entity.AfterCreateAsync(entity);
        }

        public override Task BeforeQueryAsync(IQuery<T> query)
        {
            return Entity.BeforeQueryAsync(query);
        }

        public override Task AfterQueryAsync(IQueryResult<T> queryResult)
        {
            return Entity.AfterQueryAsync(queryResult);
        }

        public override Task BeforeUpdateAsync(T old, T @new)
        {
            return Entity.BeforeUpdateAsync(old, @new);
        }

        public override Task AfterUpdateAsync(T old, T @new)
        {
            return Entity.AfterUpdateAsync(old, @new);
        }

        public override Task BeforeDeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return Entity.BeforeDeleteAsync(predicate);
        }

        public override Task AfterDeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return Entity.AfterDeleteAsync(predicate);
        }

        public override Task BeforeDeleteAllAsync()
        {
            return Entity.BeforeDeleteAllAsync();
        }

        public override Task AfterDeleteAllAsync()
        {
            return Entity.AfterDeleteAllAsync();
        }

        public override Task BeforeCountAsync(Expression<Func<T, bool>> predicate)
        {
            return Entity.BeforeCountAsync(predicate);
        }

        public override Task AfterCountAsync(Expression<Func<T, bool>> predicate)
        {
            return Entity.AfterCountAsync(predicate);
        }

        public override Task BeforeCountAllAsync()
        {
            return Entity.BeforeCountAllAsync();
        }

        public override Task AfterCountAllAsync()
        {
            return Entity.AfterCountAllAsync();
        }
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
