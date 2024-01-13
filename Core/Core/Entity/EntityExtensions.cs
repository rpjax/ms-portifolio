using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Provides extension methods for <see cref="EntityService{T}"/>.
/// </summary>
public static class EntityExtensions
{
    //*
    // TODO: Implement IVisitableQueryable
    //*

    /// <summary>
    /// Exposes the underlying <see cref="IDataAccessObject{T}"/> as an <see cref="IQueryable{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static IQueryable<T> AsQueryable<T>(this EntityService<T> entity) where T : IQueryableModel
    {
        return entity.CreateQueryAsync().Result;
    }

    /// <summary>
    /// Asynchronously creates a collection of entities.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entries">The entities to create.</param>
    /// <returns>A task that represents the asynchronous create operation.</returns>
    public static async Task CreateAsync<T>(this EntityService<T> entity, IEnumerable<T> entries) where T : IQueryableModel
    {
        var valdiationTasks = new List<Task<Exception?>>(entries.Count());

        if (entity.Validator != null)
        {
            foreach (var entry in entries)
            {
                valdiationTasks.Add(entity.Validator.ValidateAsync(entry));
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
            beforeCreateTasks.Add(entity.Hooks.BeforeCreateAsync(entry));
        }

        await Task.WhenAll(beforeCreateTasks);

        //*
        //  DataAccessObject create call. 
        //*

        await entity.DataAccessObject.InsertAsync(entries);

        //*
        // Bulk AfterCreate calls.
        //*

        var afterCreateTasks = new List<Task>(entries.Count());

        foreach (var entry in entries)
        {
            afterCreateTasks.Add(entity.Hooks.AfterCreateAsync(entry));
        }

        await Task.WhenAll(afterCreateTasks);
    }

    /// <summary>
    /// Tries to asynchronously retrieve an entity by its ID.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The retrieved entity or default if not found.</returns>
    public static async Task<T?> TryGetAsync<T>(this EntityService<T> entity, string id) where T : IQueryableModel
    {
        RunIdFormatValidation(entity, id);

        var query = CreateQueryWhereIdEquals(entity, id);
        var queryResult = await entity.QueryAsync(query);

        if (queryResult == null)
        {
            return default;
        }

        return queryResult.First;
    }

    /// <summary>
    /// Asynchronously retrieves an entity by its ID.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The retrieved entity.</returns>
    /// <exception cref="AppException">Thrown when no entity matches the provided ID.</exception>
    public static async Task<T> GetAsync<T>(this EntityService<T> entity, string id) where T : IQueryableModel
    {
        var data = await TryGetAsync(entity, id);

        if (data == null)
        {
            throw new AppException($"No entity found with the given ID: \"{id}\".", ExceptionCode.InvalidInput);
        }

        return data;
    }

    /// <summary>
    /// Performs a query with the default pagination value and no filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Task<IQueryResult<T>> QueryAsync<T>(this EntityService<T> entity) where T : IQueryableModel
    {
        return entity.QueryAsync(new Query<T>());
    }

    /// <summary>
    /// Performs a query with the provided pagination value and no filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="pagination"></param>
    /// <returns></returns>
    public static Task<IQueryResult<T>> QueryAsync<T>(this EntityService<T> entity, PaginationIn pagination) where T : IQueryableModel
    {
        return entity.QueryAsync(new Query<T>() { Pagination = pagination });
    }

    /// <summary>
    /// Asynchronously deletes an entity by its ID.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A task that represents the asynchronous delete operation.</returns>
    public static async Task DeleteAsync<T>(this EntityService<T> entity, string id) where T : IQueryableModel
    {
        if (entity.Settings.ValidateIdBeforeDeletion)
        {
            await RunIdValidationAsync(entity, id);
        }

        await entity.DeleteAsync(WhereIdEquals(entity, id));
    }

    /// <summary>
    /// Asynchronously deletes the specified instance of type <typeparamref name="T"/> from the given entity.
    /// </summary>
    /// <typeparam name="T">The type of the instance, which must implement the <see cref="IQueryableModel"/> interface.</typeparam>
    /// <param name="entity">The entity representing the data set of type <typeparamref name="T"/> from which the instance will be deleted.</param>
    /// <param name="instance">The instance to be deleted.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    /// <remarks>
    /// This method leverages the <see cref="IQueryableModel.GetId"/> method to retrieve the identifier of the instance and subsequently deletes it from the entity.
    /// </remarks>
    public static Task DeleteAsync<T>(this EntityService<T> entity, T instance) where T : IQueryableModel
    {
        return entity.DeleteAsync(instance.GetId());
    }

    /// <summary>
    /// Asynchronously deletes the instances of type <typeparamref name="T"/> from the given entity using a collection of IDs.
    /// </summary>
    /// <typeparam name="T">The type of the instance, which must implement the <see cref="IQueryableModel"/> interface.</typeparam>
    /// <param name="entity">The entity representing the data set of type <typeparamref name="T"/>.</param>
    /// <param name="ids">An enumeration of identifiers representing the instances to be deleted.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous deletion operation.</returns>
    /// <remarks>
    /// If the entity's <c>ValidateIdBeforeDeletion</c> property is set to true, this method will validate the IDs asynchronously before deletion.
    /// </remarks>
    public static async Task DeleteAsync<T>(this EntityService<T> entity, IEnumerable<string> ids) where T : IQueryableModel
    {
        if (ids.IsEmpty())
        {
            return;
        }

        if (entity.Settings.ValidateIdBeforeDeletion)
        {
            var tasks = new List<Task>(ids.Count());

            foreach (string id in ids)
            {
                tasks.Add(entity.RunIdValidationAsync(id));
            }

            await Task.WhenAll(tasks);
        }

        Expression<Func<T, bool>>? expression = null;

        foreach (string id in ids)
        {
            if (expression == null)
            {
                expression = entity.WhereIdEquals(id);
            }
            else
            {
                var body = Expression.OrElse(expression.Body, entity.IdEquals(id));
                expression = Expression.Lambda<Func<T, bool>>(body, expression.Parameters);
            }

            expression = Expression.Lambda<Func<T, bool>>(expression.Body, expression!.Parameters);
        }

        await entity.DeleteAsync(expression!);
    }

    /// <summary>
    /// Runs the ID format validation for the provided ID. Throws an exception if the ID is malformed.
    /// </summary>
    /// <typeparam name="T">The type of the instance, which must implement the <see cref="IQueryableModel"/> interface.</typeparam>
    /// <param name="entity">The entity representing the data set of type <typeparamref name="T"/>.</param>
    /// <param name="id">The identifier to be validated.</param>
    /// <exception cref="AppException">Thrown when the provided ID is malformed.</exception>
    public static void RunIdFormatValidation<T>(this EntityService<T> entity, string id) where T : IQueryableModel
    {
        var isValid = entity.DataAccessObject.ValidateIdFormat(id);

        if (!isValid)
        {
            throw new AppException($"Malformed {typeof(T).FullName} ID.", ExceptionCode.InvalidInput);
        }
    }

    /// <summary>
    /// Asynchronously runs the ID validation for the provided ID. Throws an exception if the ID is invalid or malformed. An entry with this ID has to exist in order for the ID to be valid.
    /// </summary>
    /// <typeparam name="T">The type of the instance, which must implement the <see cref="IQueryableModel"/> interface.</typeparam>
    /// <param name="entity">The entity representing the data set of type <typeparamref name="T"/>.</param>
    /// <param name="id">The identifier to be validated.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous validation operation.</returns>
    /// <exception cref="AppException">Thrown when the provided ID is invalid or malformed.</exception>
    public static async Task RunIdValidationAsync<T>(this EntityService<T> entity, string id) where T : IQueryableModel
    {
        var isValid = await entity.ValidateIdAsync(id);

        if (!isValid)
        {
            throw new AppException($"Malformed or invalid {typeof(T).Name} ID.", ExceptionCode.InvalidInput);
        }
    }

    /// <summary>
    /// Asynchronously counts the instances of type <typeparamref name="T"/> from the given entity that matches the provided ID.
    /// </summary>
    /// <typeparam name="T">The type of the instance, which must implement the <see cref="IQueryableModel"/> interface.</typeparam>
    /// <param name="entity">The entity representing the data set of type <typeparamref name="T"/>.</param>
    /// <param name="id">The identifier used for filtering and counting the instances.</param>
    /// <returns>A <see cref="Task"/> that results in the count of instances that match the provided ID.</returns>
    public static async Task<long> CountAsync<T>(this EntityService<T> entity, string id) where T : IQueryableModel
    {
        await entity.RunIdValidationAsync(id);
        return await entity.DataAccessObject.CountAsync(entity.WhereIdEquals(id));
    }

    //*
    // TREE VISITORS.
    //*

    /// <summary>
    /// Performs mapping of interfaces to their concrete implementations.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="expression"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull("expression")]
    public static Expression<TDelegate>? Visit<T, TDelegate>(this EntityService<T> entity, Expression<TDelegate>? expression) where T : IQueryableModel
    {
        var visited = entity.Visit(expression);

        if (visited != null)
        {
            return visited.TypeCast<Expression<TDelegate>>();
        }

        return null;
    }

    //*
    // EXPRESSION GENERATORS
    //*

    /// <summary>
    /// Creates an expression that represents the LINQ for: "<typeparamref name="T"/>.$Id == $<paramref name="id"/>".<br/>
    /// Note that this method returns a <see cref="BinaryExpression"/> for <see cref="ExpressionType.Equal"/>.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="id"></param>
    /// <param name="visit"></param>
    /// <returns></returns>
    public static Expression IdEquals<T>(this EntityService<T> entity, string id, bool visit = false) where T : IQueryableModel
    {
        Expression<Func<T, bool>> expression = (T x) => EntityLinq.IdEqualsFlag(id);

        if (visit)
        {
            return Visit(entity, expression).Body;
        }

        return expression.Body;
    }

    /// <summary>
    /// Creates an expression that represents the LINQ for: "(<typeparamref name="T"/> obj) => obj.$Id == $<paramref name="id"/>".
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="id"></param>
    /// <param name="visit"></param>
    /// <returns></returns>
    public static Expression<Func<T, bool>> WhereIdEquals<T>(this EntityService<T> entity, string id, bool visit = true) where T : IQueryableModel
    {
        Expression<Func<T, bool>> expression = (T x) => EntityLinq.IdEqualsFlag(id);

        if (visit)
        {
            return Visit(entity, expression);
        }

        return expression;
    }

    /// <summary>
    /// Creates a query with a LINQ filter for: "(<typeparamref name="T"/> obj) => obj.$Id == $<paramref name="id"/>".
    /// </summary>
    /// <param name="service"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static IQuery<T> CreateQueryWhereIdEquals<T>(this EntityService<T> service, string id) where T : IQueryableModel
    {
        var pagination = new PaginationIn(1, 0);
        var writer = new QueryWriter<T>();

        return writer
            .SetPagination(pagination)
            .SetFilter(service.WhereIdEquals(id))
            .Create();
    }

    /// <summary>
    /// Asynchronously validates the specified entity using the service's asynchronous validator.
    /// </summary>
    /// <param name="service">The service that contains the validator.</param>
    /// <param name="entity">The entity to validate.</param>
    /// <typeparam name="T">The type of the entity, which must implement IQueryableModel.</typeparam>
    /// <returns>
    /// A task that represents the asynchronous validation operation. The task result contains the 
    /// <see cref="ValidationResult"/> of the validation. <br/>
    /// If the service has no asynchronous validator, a successful validation result is returned.
    /// </returns>
    public static Task<ValidationResult> ValidateAsync<T>(this EntityService<T> service, T entity) where T : IQueryableModel
    {
        if(service.AsyncValidator != null)
        {
            return service.AsyncValidator.ValidateAsync(entity);    
        }

        return Task.FromResult(new ValidationResult());
    }

    /// <summary>
    /// Asynchronously validates the entity for an update operation using the service's asynchronous update validator.
    /// </summary>
    /// <param name="service">The service containing the update validator.</param>
    /// <param name="entity">The entity to be validated for update.</param>
    /// <typeparam name="T">The type of the entity which must implement IQueryableModel.</typeparam>
    /// <returns>
    /// A task representing the asynchronous update validation operation. The task result contains the 
    /// <see cref="ValidationResult"/> with the outcome of the validation. <br/>
    /// If no asynchronous update validator is provided by the service, a successful validation result is returned.
    /// </returns>
    public static Task<ValidationResult> ValidateUpdateAsync<T>(this EntityService<T> service, T entity) where T : IQueryableModel
    {
        if (service.AsyncUpdateValidator != null)
        {
            return service.AsyncUpdateValidator.ValidateAsync(entity);
        }

        return Task.FromResult(new ValidationResult());
    }

    /// <summary>
    /// Asynchronously validates a query against the entity using the service's asynchronous query validator.
    /// </summary>
    /// <param name="service">The service containing the query validator.</param>
    /// <param name="query">The query object to validate.</param>
    /// <typeparam name="T">The type of the entity which must implement IQueryableModel.</typeparam>
    /// <returns>
    /// A task representing the asynchronous query validation operation. The task result contains the 
    /// <see cref="ValidationResult"/> indicating the outcome of the validation. <br/>
    /// If the service does not have an asynchronous query validator, a successful validation result is returned.
    /// </returns>
    public static Task<ValidationResult> ValidateQueryAsync<T>(this EntityService<T> service, IQuery<T> query) where T : IQueryableModel
    {
        if (service.AsyncQueryValidator != null)
        {
            return service.AsyncQueryValidator.ValidateAsync(query);
        }

        return Task.FromResult(new ValidationResult());
    }

}
