using ModularSystem.Core.Expressions;
using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Core;

public class EnititySettings
{
    public bool ValidateIdBeforeDeletion { get; set; }
    public bool UseValidators { get; set; }
}

/// <summary>
/// Abstract base class for entities, providing shared CRUD operations, serialization, and expression handling.
/// </summary>
/// <typeparam name="T">The type of the entity being operated on, which must implement IQueryableModel.</typeparam>
public abstract class EntityService<T> : IEntityService<T> where T : IQueryableModel
{
    /// <summary>
    /// Gets or sets the settings associated with the entity.
    /// </summary>
    public EnititySettings Settings { get; set; } 

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
    /// Gets the asynchronous validator used for the entity. If no asynchronous validator is provided,
    /// the entity won't be validated asynchronously.
    /// </summary>
    public IAsyncValidator<T>? AsyncValidator { get; init; }

    /// <summary>
    /// Gets the asynchronous validator used for updating the entity. If no asynchronous update validator is provided,
    /// updates to the entity won't be validated asynchronously.
    /// </summary>
    public IAsyncValidator<T>? AsyncUpdateValidator { get; init; }

    /// <summary>
    /// Gets the asynchronous validator used for querying the entity. If no asynchronous query validator is provided,
    /// queries won't be validated asynchronously.
    /// </summary>
    public IAsyncValidator<IQuery<T>>? AsyncQueryValidator { get; init; }

    /// <summary>
    /// Retrieves an instance of a middleware wrapper that encapsulates the hooks of the current entity.
    /// </summary>
    /// <returns>An instance representing the hooks of the entity.</returns>
    public EntityMiddleware<T> Hooks => new HooksWrapper(this);

    /// <summary>
    /// A collection of middlewares to be executed by the entity.
    /// </summary>
    private ConcurrentQueue<EntityMiddleware<T>> Middlewares { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityService{T}"/> class.
    /// </summary>
    protected EntityService()
    {
        Settings = new();
        Validator = null;
        UpdateValidator = null;
        QueryValidator = null;
        Middlewares = new();
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
        Middlewares.Enqueue(middleware);
    }

    /// <summary>
    /// Adds a middleware of the specified type to the list of middlewares for this entity.
    /// </summary>
    /// <typeparam name="TMiddleware">The type of middleware to add.</typeparam>
    public void AddMiddleware<TMiddleware>() where TMiddleware : EntityMiddleware<T>, new()
    {
        Middlewares.Enqueue(new TMiddleware());
    }

    /// <summary>
    /// Clears all middlewares from the list.
    /// </summary>
    public void ClearMiddlewares()
    {
        Middlewares.Clear();
    }

    /// <summary>
    /// Registers a specified expression visitor as middleware to inspect and potentially modify entity-related expressions.
    /// </summary>
    /// <param name="visitor">An instance of <see cref="EntityExpressionVisitor{T}"/> to be used as middleware.</param>
    public void AddExpressionVisitor(EntityExpressionVisitor<T> visitor)
    {
        AddMiddleware(new VisitorMiddlewareConverter<T>(visitor));
    }

    /// <summary>
    /// Registers an expression visitor of the specified type as middleware. 
    /// This variant initializes a new instance of the visitor.
    /// </summary>
    /// <typeparam name="TVisitor">The type of the expression visitor derived from <see cref="EntityExpressionVisitor{T}"/>. The type should have a parameterless constructor.</typeparam>
    public void AddExpressionVisitor<TVisitor>() where TVisitor : EntityExpressionVisitor<T>, new()
    {
        AddExpressionVisitor(new TVisitor());
    }

    /// <summary>
    /// Constructs a comprehensive middleware pipeline by combining pre-user, user-defined, and post-user middlewares. <br/>
    /// </summary>
    /// <remarks>
    /// The method merges three sequences of middlewares:<br/>
    /// 1. Those generated by <see cref="CreatePreUserPipeline"/>.<br/>
    /// 2. The user-defined middlewares maintained in the 'Middlewares' collection.<br/>
    /// 3. Those generated by <see cref="CreatePostUserPipeline"/>.
    /// </remarks>
    /// <returns>An aggregated sequence of all middlewares to be executed in the established order.</returns>
    public virtual IEnumerable<EntityMiddleware<T>> CreateMiddlewarePipeline()
    {
        foreach (var middleware in CreatePreUserPipeline())
        {
            yield return middleware;
        }

        foreach (var middleware in Middlewares)
        {
            yield return middleware;
        }

        foreach (var middleware in CreatePostUserPipeline())
        {
            yield return middleware;
        }
    }

    /// <summary>
    /// Factory method to create an expression visitor for the entity.
    /// </summary>
    /// <returns>A new instance of <see cref="IVisitor{Expression}"/> tailored for this entity.</returns>
    public virtual IVisitor<Expression> CreateExpressionNormalizer()
    {
        return new EntityLinqNormalizerVisitor<T>(CreateIdSelectorExpression, ParseId);
    }

    //*
    // CREATE.
    //*

    /// <summary>
    /// Asynchronously creates a single new entity, invoking the associated middlewares before and after the creation process.<br/>
    /// </summary>
    /// <remarks>
    /// The method follows these steps:<br/>
    /// 1. Invokes the <see cref="BeforeCreateAsync(T)"/> method for the entity, potentially transforming it before creation.<br/>
    /// 2. Persists the transformed entity using the underlying data access object's insert operation.<br/>
    /// 3. After the entity has been persisted, invokes the <see cref="AfterCreateAsync(T)"/> method to potentially post-process the entity.<br/>
    /// </remarks>
    /// <param name="entry">The entity to be created.</param>
    /// <returns>The ID of the created entity.</returns>
    public virtual async Task<string> CreateAsync(T entry)
    {
        entry = await BeforeCreateAsync(entry);
        await DataAccessObject.InsertAsync(entry);
        entry = await AfterCreateAsync(entry);

        return entry.GetId();
    }

    /// <summary>
    /// Asynchronously creates a collection of entities, invoking the associated middlewares before and after the creation process.<br/>
    /// </summary>
    /// <remarks>
    /// The method executes the creation process in the following sequence:<br/>
    /// 1. Invokes the <see cref="BeforeCreateAsync(IEnumerable{T})"/> method for each entity, potentially transforming the entities before creation.<br/>
    /// 2. Persists the transformed entities using the underlying data access object's bulk insert operation.<br/>
    /// 3. After the entities have been persisted, invokes the <see cref="AfterCreateAsync(IEnumerable{T})"/> method to potentially post-process the entities.<br/>
    /// </remarks>
    /// <param name="entities">The collection of entities to be created.</param>
    /// <returns>An array of string identifiers corresponding to the created entities.</returns>
    public async Task<string[]> CreateAsync(IEnumerable<T> entities)
    {
        entities = await BeforeCreateAsync(entities);
        await DataAccessObject.InsertAsync(entities);
        entities = await AfterCreateAsync(entities);

        return entities.Select(x => x.GetId()).ToArray();
    }

    //*
    // READ.
    //*

    /// <summary>
    /// Provides an asynchronous mechanism to generate an initial query for entities of type <typeparamref name="T"/>. The returned query is flexible and can be further refined or filtered using LINQ. Execution is deferred until the query is materialized, e.g., by invoking ToList() or ToArray().
    /// </summary>
    /// <remarks>
    /// This method serves as an entry point to construct dynamic queries for entities. It internally calls <see cref="OnCreateQueryAsync"/> to allow middleware components to potentially alter the query before it's returned.
    /// </remarks>
    /// <returns>An IQueryable of type <typeparamref name="T"/> which can be further shaped using LINQ.</returns>
    public Task<IQueryable<T>> CreateQueryableAsync()
    {
        return OnCreateQueryAsync(DataAccessObject.AsQueryable());
    }

    /// <summary>
    /// Asynchronously retrieves entities based on the provided query criteria, invoking associated middlewares before and after the query process.<br/>
    /// </summary>
    /// <remarks>
    /// The method unfolds as follows:<br/>
    /// 1. Invokes the <see cref="BeforeQueryAsync"/> method for the provided query, allowing for potential modifications or validations of the query criteria.<br/>
    /// 2. Executes the query against the underlying data access object, potentially applying any query visitors during the process.<br/>
    /// 3. After retrieving the results, invokes the <see cref="AfterQueryAsync"/> method, offering a chance for post-query operations or transformations on the result set.<br/>
    /// </remarks>
    /// <param name="query">The query criteria to filter and fetch the entities.</param>
    /// <returns>The results obtained from the query execution.</returns>
    public virtual async Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        query = await BeforeQueryAsync(query);
        var queryResult = await DataAccessObject.QueryAsync(query);
        queryResult = await AfterQueryAsync(queryResult);

        return queryResult;
    }

    //*
    // UPDATE.
    //* 

    /// <summary>
    /// Asynchronously updates an entity, invoking associated middlewares before and after the update process.<br/>
    /// </summary>
    /// <remarks>
    /// The update process is sequenced as follows:<br/>
    /// 1. Retrieves the current state of the entity using its identifier.<br/>
    /// 2. Invokes the <see cref="BeforeUpdateAsync(T, T)"/> method with both the current and updated entities, allowing for potential modifications, validations, or side-effects before the update.<br/>
    /// 3. Executes the update against the underlying data access object.<br/>
    /// 4. Invokes the <see cref="AfterUpdateAsync(T, T)"/> method, offering a chance for post-update operations or transformations on the updated entity.<br/>
    /// </remarks>
    /// <param name="updatedValue">The entity with the desired new values.</param>
    /// <returns>A task that represents the asynchronous update operation.</returns>
    public virtual async Task UpdateAsync(T updatedValue)
    {
        var currentValue = await this.GetAsync(updatedValue.GetId());

        (currentValue, updatedValue) = await BeforeUpdateAsync(currentValue, updatedValue);
        await DataAccessObject.UpdateAsync(updatedValue);
        (currentValue, updatedValue) = await AfterUpdateAsync(currentValue, updatedValue);
    }

    /// <summary>
    /// Asynchronously updates entities based on the provided update criteria and values.
    /// </summary>
    /// <remarks>
    /// The method orchestrates the update process in the following sequence: <br/>
    /// 1. Invokes the <see cref="BeforeUpdateAsync(IUpdate{T})"/> to perform actions prior to the actual update.<br/>
    /// 2. Updates the entities using the underlying data access object.<br/>
    /// 3. Invokes the <see cref="AfterUpdateAsync(IUpdate{T})"/> to perform actions after the update has been applied.
    /// </remarks>
    /// <param name="update">The update criteria and values to be applied.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public virtual async Task<long?> UpdateAsync(IUpdate<T> update)
    {
        await BeforeUpdateAsync(update);
        var affectedRecordsCount = await DataAccessObject.UpdateAsync(update);
        await AfterUpdateAsync(update);
        return affectedRecordsCount;
    }

    //*
    // DELETE.
    //*

    /// <summary>
    /// Asynchronously deletes entities based on the provided predicate, invoking associated middlewares before and after the deletion process.<br/>
    /// </summary>
    /// <remarks>
    /// The deletion process unfolds in the following sequence:<br/>
    /// 1. The provided predicate undergoes potential modifications or validations through the <see cref="BeforeDeleteAsync"/> method.<br/>
    /// 2. Utilizing the potentially modified predicate, the method deletes matching entities from the underlying data access object.<br/>
    /// 3. Executes the <see cref="AfterDeleteAsync"/> method, allowing for post-deletion operations or effects based on the initial predicate.<br/>
    /// </remarks>
    /// <param name="predicate">The expression determining which entities should be targeted for deletion.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public virtual async Task<long?> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        predicate = await BeforeDeleteAsync(predicate);
        var affectedRecordsCount = await DataAccessObject.DeleteAsync(this.Visit<T, Func<T, bool>>(predicate));
        await AfterDeleteAsync(predicate);
        return affectedRecordsCount;
    }

    /// <summary>
    /// Asynchronously purges all entities from the data storage, invoking associated middlewares before and after the deletion process.<br/>
    /// </summary>
    /// <remarks>
    /// The process of deletion follows these steps:<br/>
    /// 1. Verification of the provided confirmation. If <paramref name="confirm"/> is not <c>true</c>, the method terminates immediately without effect.<br/>
    /// 2. Invokes the <see cref="BeforeDeleteAllAsync"/> method to handle any pre-deletion operations or validations.<br/>
    /// 3. Deletes all entities from the underlying data access object.<br/>
    /// 4. Executes the <see cref="AfterDeleteAllAsync"/> method, allowing for any post-deletion operations or notifications.<br/>
    /// <strong>Note:</strong> Using this operation without caution may lead to irreversible data loss. Ensure adequate backups and validations are in place before executing.
    /// </remarks>
    /// <param name="confirm">A flag to confirm the intention to delete all entities. If not set to <c>true</c>, no action will be taken.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
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
        predicate = await BeforeCountAsync(predicate);
        var count = await DataAccessObject.CountAsync(predicate);
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
    /// This method delegates the visitation to a custom expression visitor created by the <see cref="CreateExpressionNormalizer"/> method.
    /// </summary>
    /// <param name="expression">The expression to be visited.</param>
    /// <returns>The visited expression with interfaces replaced by their concrete implementations if applicable, or null if the provided expression is null.</returns>
    [return: NotNullIfNotNull("expression")]
    public virtual Expression? Visit(Expression? expression)
    {
        return CreateExpressionNormalizer().Visit(expression);
    }

    //*
    // private section
    //*

    private IEnumerable<EntityMiddleware<T>> CreatePreUserPipeline()
    {
        return Enumerable.Empty<EntityMiddleware<T>>();
    }

    private IEnumerable<EntityMiddleware<T>> CreatePostUserPipeline()
    {
        if(Settings.UseValidators)
        {
            yield return new ValidationMiddleware<T>(this);
        }

        yield return new VisitorMiddlewareConverter<T>(new ExpressionNormalizer<T>(CreateIdSelectorExpression, ParseId));
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
    // query creation hooks
    //*

    /// <summary>
    /// Called when constructing a new queryable object for entities of type <typeparamref name="T"/>. This method allows middleware components to potentially alter the query before it is returned.
    /// </summary>
    /// <remarks>
    /// The method iterates through each middleware defined by the <see cref="CreateMiddlewarePipeline"/> and invokes their <c>OnCreateQueryAsync</c> methods, allowing for modifications to the initial query.
    /// </remarks>
    /// <param name="queryable">The initial queryable object representing the entities.</param>
    /// <returns>An altered or unaltered IQueryable, depending on middleware operations.</returns>
    protected virtual async Task<IQueryable<T>> OnCreateQueryAsync(IQueryable<T> queryable)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            queryable = await middleware.OnCreateQueryAsync(queryable);
        }

        return queryable;
    }

    //*
    // before validation hook
    //*

    /// <summary>
    /// Called before the validation of the entity.
    /// </summary>
    /// <param name="entity">The entity to be validated.</param>
    protected virtual async Task<T> BeforeValidateAsync(T entity)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            entity = await middleware.BeforeValidateAsync(entity);
        }

        return entity;
    }

    //*
    // creation hooks
    //*

    /// <summary>
    /// Called before creating the entity.
    /// </summary>
    /// <param name="entity">The entity to be created.</param>
    protected virtual async Task<T> BeforeCreateAsync(T entity)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            entity = await middleware.BeforeCreateAsync(entity);
        }

        return entity;
    }

    /// <summary>
    /// Called after creating the entity.
    /// </summary>
    /// <param name="entity">The recently created entity.</param>
    protected virtual async Task<T> AfterCreateAsync(T entity)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            await middleware.AfterCreateAsync(entity);
        }

        return entity;
    }

    /// <summary>
    /// Asynchronously processes a collection of entities prior to their creation, invoking each middleware's 'BeforeCreateAsync' method in sequence.<br/>
    /// </summary>
    /// <remarks>
    /// The pre-processing is driven by the sequence of middlewares generated by the <see cref="CreateMiddlewarePipeline"/> method.<br/>
    /// This method can be used to perform any requisite operations, checks, or transformations on entities before they're persisted.
    /// </remarks>
    /// <param name="entities">The collection of entities set for creation.</param>
    /// <returns>The potentially modified collection of entities after processing through all middlewares.</returns>
    protected virtual async Task<IEnumerable<T>> BeforeCreateAsync(IEnumerable<T> entities)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            entities = await middleware.BeforeCreateAsync(entities);
        }

        return entities;
    }

    /// <summary>
    /// Asynchronously processes a collection of entities following their creation, invoking each middleware's 'AfterCreateAsync' method in sequence.<br/>
    /// </summary>
    /// <remarks>
    /// The post-processing is determined by the sequence of middlewares generated by the <see cref="CreateMiddlewarePipeline"/> method.<br/>
    /// This method can be employed to execute post-persistence operations, such as notifying other systems, caching, or any additional data handling.
    /// </remarks>
    /// <param name="entities">The collection of recently created entities.</param>
    /// <returns>The potentially modified collection of entities after processing through all middlewares.</returns>
    protected virtual async Task<IEnumerable<T>> AfterCreateAsync(IEnumerable<T> entities)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            entities = await middleware.AfterCreateAsync(entities);
        }

        return entities;
    }

    //*
    // query hooks
    //*

    /// <summary>
    /// Called before querying the entity.
    /// </summary>
    /// <param name="query">The query to be executed.</param>
    protected virtual async Task<IQuery<T>> BeforeQueryAsync(IQuery<T> query)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            query = await middleware.BeforeQueryAsync(query);
        }

        return query;
    }

    /// <summary>
    /// Called after querying the entity.
    /// </summary>
    /// <param name="queryResult">The result of the query.</param>
    protected virtual async Task<IQueryResult<T>> AfterQueryAsync(IQueryResult<T> queryResult)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            queryResult = await middleware.AfterQueryAsync(queryResult);
        }

        return queryResult;
    }

    //*
    // update hooks
    //*

    /// <summary>
    /// Asynchronously processes the update parameters prior to executing an update, invoking each middleware's 'BeforeUpdateAsync' method in sequence.<br/>
    /// </summary>
    /// <remarks>
    /// The pre-processing is governed by the sequence of middlewares produced by the <see cref="CreateMiddlewarePipeline"/> method.<br/>
    /// This method allows for operations, validations, or transformations on the update parameters before they're applied to entities.
    /// </remarks>
    /// <param name="update">The update parameters detailing the conditions and changes for the update.</param>
    /// <returns>The potentially modified update parameters after processing through all middlewares.</returns>
    protected virtual async Task<IUpdate<T>> BeforeUpdateAsync(IUpdate<T> update)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            update = await middleware.BeforeUpdateAsync(update);
        }

        return update;
    }

    /// <summary>
    /// Asynchronously processes the update parameters following the execution of an update, invoking each middleware's 'AfterUpdateAsync' method in sequence.<br/>
    /// </summary>
    /// <remarks>
    /// The post-processing is driven by the sequence of middlewares outlined by the <see cref="CreateMiddlewarePipeline"/> method.<br/>
    /// This method facilitates post-update operations, such as auditing, notifying other systems, or refreshing caches.
    /// </remarks>
    /// <param name="update">The update parameters that were applied during the update operation.</param>
    /// <returns>The potentially modified update parameters after processing through all middlewares.</returns>
    protected virtual async Task<IUpdate<T>> AfterUpdateAsync(IUpdate<T> update)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            update = await middleware.AfterUpdateAsync(update);
        }

        return update;
    }

    /// <summary>
    /// Called before updating the entity.<br/>
    /// By default, this method retains the values of <see cref="IQueryableModel.CreatedAt"/> from <paramref name="currentValue"/> to <paramref name="updatedValue"/>,<br/>
    /// and sets <see cref="IQueryableModel.LastModifiedAt"/> to <see cref="TimeProvider.UtcNow"/>.
    /// </summary>
    /// <param name="currentValue">The current state of the entity.</param>
    /// <param name="updatedValue">The new state of the entity.</param>
    protected virtual async Task<(T, T)> BeforeUpdateAsync(T currentValue, T updatedValue)
    {
        updatedValue.CreatedAt = currentValue.CreatedAt;
        updatedValue.LastModifiedAt = TimeProvider.UtcNow();

        foreach (var middleware in CreateMiddlewarePipeline())
        {
            (currentValue, updatedValue) = await middleware.BeforeUpdateAsync(currentValue, updatedValue);
        }

        return (currentValue, updatedValue);
    }

    /// <summary>
    /// Called after updating the entity.
    /// </summary>
    /// <param name="currentValue">The state of the entity before the update.</param>
    /// <param name="updatedValue">The updated state of the entity.</param>
    protected virtual async Task<(T, T)> AfterUpdateAsync(T currentValue, T updatedValue)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            (currentValue, updatedValue) = await middleware.AfterUpdateAsync(currentValue, updatedValue);
        }

        return (currentValue, updatedValue);
    }

    //*
    // deletion hooks
    //*

    /// <summary>
    /// Called before deleting the entity based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate used for the delete operation.</param>
    protected virtual async Task<Expression<Func<T, bool>>> BeforeDeleteAsync(Expression<Func<T, bool>> predicate)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            predicate = await middleware.BeforeDeleteAsync(predicate);
        }

        return predicate;
    }

    /// <summary>
    /// Called after deleting the entity based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate used for the delete operation.</param>
    protected virtual async Task<Expression<Func<T, bool>>> AfterDeleteAsync(Expression<Func<T, bool>> predicate)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            predicate = await middleware.AfterDeleteAsync(predicate);
        }

        return predicate;
    }

    //*
    // delete all hooks
    //*

    /// <summary>
    /// Called before deleting all entities.
    /// </summary>
    protected virtual async Task BeforeDeleteAllAsync()
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            await middleware.BeforeDeleteAllAsync();
        }
    }

    /// <summary>
    /// Called after deleting all entities.
    /// </summary>
    protected virtual async Task AfterDeleteAllAsync()
    {
        foreach (var middleware in CreateMiddlewarePipeline())
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
    protected virtual async Task<Expression<Func<T, bool>>> BeforeCountAsync(Expression<Func<T, bool>> predicate)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            predicate = await middleware.BeforeCountAsync(predicate);
        }

        return predicate;
    }

    /// <summary>
    /// Called after counting the entities based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate used for the count operation.</param>
    protected virtual async Task<Expression<Func<T, bool>>> AfterCountAsync(Expression<Func<T, bool>> predicate)
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            predicate = await middleware.AfterCountAsync(predicate);
        }

        return predicate;
    }

    //*
    // count all hooks
    //*

    /// <summary>
    /// Called before counting all entities.
    /// </summary>
    protected virtual async Task BeforeCountAllAsync()
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            await middleware.BeforeCountAllAsync();
        }
    }

    /// <summary>
    /// Called after counting all entities.
    /// </summary>
    protected virtual async Task AfterCountAllAsync()
    {
        foreach (var middleware in CreateMiddlewarePipeline())
        {
            await middleware.AfterCountAllAsync();
        }
    }

    internal class HooksWrapper : EntityMiddleware<T>
    {
        private EntityService<T> Entity { get; }

        public HooksWrapper(EntityService<T> entity)
        {
            Entity = entity;
        }

        public override Task<T> BeforeValidateAsync(T entity)
        {
            return Entity.BeforeValidateAsync(entity);
        }

        public override Task<T> BeforeCreateAsync(T entity)
        {
            return Entity.BeforeCreateAsync(entity);
        }

        public override Task<T> AfterCreateAsync(T entity)
        {
            return Entity.AfterCreateAsync(entity);
        }

        public override Task<IEnumerable<T>> BeforeCreateAsync(IEnumerable<T> entities)
        {
            return Entity.BeforeCreateAsync(entities);
        }

        public override Task<IEnumerable<T>> AfterCreateAsync(IEnumerable<T> entities)
        {
            return Entity.AfterCreateAsync(entities);
        }

        public override Task<IQueryable<T>> OnCreateQueryAsync(IQueryable<T> queryable)
        {
            return Entity.OnCreateQueryAsync(queryable);
        }

        public override Task<IQuery<T>> BeforeQueryAsync(IQuery<T> query)
        {
            return Entity.BeforeQueryAsync(query);
        }

        public override Task<IQueryResult<T>> AfterQueryAsync(IQueryResult<T> queryResult)
        {
            return Entity.AfterQueryAsync(queryResult);
        }

        public override Task<(T, T)> BeforeUpdateAsync(T old, T @new)
        {
            return Entity.BeforeUpdateAsync(old, @new);
        }

        public override Task<(T, T)> AfterUpdateAsync(T old, T @new)
        {
            return Entity.AfterUpdateAsync(old, @new);
        }

        public override Task<IUpdate<T>> BeforeUpdateAsync(IUpdate<T> update)
        {
            return Entity.BeforeUpdateAsync(update);
        }

        public override Task<IUpdate<T>> AfterUpdateAsync(IUpdate<T> update)
        {
            return Entity.AfterUpdateAsync(update);
        }

        public override Task<Expression<Func<T, bool>>> BeforeDeleteAsync(Expression<Func<T, bool>> predicate)
        {
            return Entity.BeforeDeleteAsync(predicate);
        }

        public override Task<Expression<Func<T, bool>>> AfterDeleteAsync(Expression<Func<T, bool>> predicate)
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

        public override Task<Expression<Func<T, bool>>> BeforeCountAsync(Expression<Func<T, bool>> predicate)
        {
            return Entity.BeforeCountAsync(predicate);
        }

        public override Task<Expression<Func<T, bool>>> AfterCountAsync(Expression<Func<T, bool>> predicate)
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
