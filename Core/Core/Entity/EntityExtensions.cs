using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Provides extension methods for <see cref="Entity{T}"/>.
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
    public static IQueryable<T> AsQueryable<T>(this Entity<T> entity) where T : IQueryableModel
    {
        return entity.DataAccessObject.AsQueryable();
    }

    /// <summary>
    /// Performs a query with the default pagination value and no filter.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static Task<IQueryResult<T>> QueryAsync<T>(this Entity<T> entity) where T : IQueryableModel
    {
        return entity.QueryAsync(new Query<T>());
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
    public static Task DeleteAsync<T>(this Entity<T> entity, T instance) where T : IQueryableModel
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
    public static async Task DeleteAsync<T>(this Entity<T> entity, IEnumerable<string> ids) where T : IQueryableModel
    {
        if (ids.IsEmpty())
        {
            return;
        }

        if (entity.ValidateIdBeforeDeletion)
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
    public static void RunIdFormatValidation<T>(this Entity<T> entity, string id) where T : IQueryableModel
    {
        var isValid = entity.DataAccessObject.ValidateIdFormat(id);

        if (!isValid)
        {
            throw new AppException($"Malformed {typeof(T).FullName} ID.", ExceptionCode.InvalidInput);
        }
    }

    /// <summary>
    /// Asynchronously runs the ID validation for the provided ID. Throws an exception if the ID is invalid or malformed.
    /// </summary>
    /// <typeparam name="T">The type of the instance, which must implement the <see cref="IQueryableModel"/> interface.</typeparam>
    /// <param name="entity">The entity representing the data set of type <typeparamref name="T"/>.</param>
    /// <param name="id">The identifier to be validated.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous validation operation.</returns>
    /// <exception cref="AppException">Thrown when the provided ID is invalid or malformed.</exception>
    public static async Task RunIdValidationAsync<T>(this Entity<T> entity, string id) where T : IQueryableModel
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
    public static async Task<long> CountAsync<T>(this Entity<T> entity, string id) where T : IQueryableModel
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
    public static Expression<TDelegate>? Visit<T, TDelegate>(this Entity<T> entity, Expression<TDelegate>? expression) where T : IQueryableModel
    {
        var visited = entity.Visit(expression);

        if (visited != null)
        {
            return visited.TypeCast<Expression<TDelegate>>();
        }

        return null;
    }

    /// <summary>
    /// Helper method to visit the expressions inside <see cref="IQuery{T}"/>.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    [return: NotNullIfNotNull("query")]
    public static IQuery<T>? Visit<T>(this Entity<T> entity, IQuery<T>? query) where T : IQueryableModel
    {
        if (query == null)
        {
            return null;
        }

        query.Filter = Visit(entity, query.Filter);
        query.Sort = Visit(entity, query.Sort);

        return query;
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
    public static Expression IdEquals<T>(this Entity<T> entity, string id, bool visit = false) where T : IQueryableModel
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
    public static Expression<Func<T, bool>> WhereIdEquals<T>(this Entity<T> entity, string id, bool visit = true) where T : IQueryableModel
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
    /// <param name="entity"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public static IQuery<T> WhereIdEqualsQuery<T>(this Entity<T> entity, string id) where T : IQueryableModel
    {
        var pagination = new PaginationIn(1, 0);
        return new Query<T>(pagination, entity.WhereIdEquals(id));
    }

    /// <summary>
    /// Retrieves the <see cref="ISerializer{T}"/> associated with the provided entity of type <typeparamref name="T"/>.
    /// This method depends on the prior invocation of <see cref="EntityInitializer"/>.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the serializer is being retrieved.</typeparam>
    /// <param name="entity">The instance of the entity.</param>
    /// <returns>An instance of <see cref="ISerializer{T}"/> if found; otherwise, null.</returns>
    public static ISerializer<T>? TryGetSerializer<T>(this Entity<T> entity) where T : class, IQueryableModel
    {
        var configuration = EntityConfiguration.TryGetConfiguration(entity.GetType());
        var typedConfiguration = configuration?.TryTypeCast<EntityConfiguration<T>>();
        var serializer = typedConfiguration?.GetSerializer();

        return serializer;
    }

    /// <summary>
    /// Retrieves the <see cref="ISerializer{T}"/> associated with the specified entity type from the <see cref="EntityConfiguration"/> container.
    /// This method depends on the prior invocation of <see cref="EntityInitializer"/> and is optimized to be slightly faster than its counterpart <see cref="TryGetSerializer{T}(Entity{T})"/> by avoiding some runtime reflection operations.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the serializer is being retrieved.</typeparam>
    /// <param name="entity">The instance of the entity (can be null).</param>
    /// <param name="entityType">The concrete type of the entity.</param>
    /// <returns>An instance of <see cref="ISerializer{T}"/> if found; otherwise, null.</returns>
    public static ISerializer<T>? TryGetSerializer<T>(this Entity<T>? entity, Type entityType) where T : class, IQueryableModel
    {
        var configuration = EntityConfiguration.TryGetConfiguration(entityType);
        var typedConfiguration = configuration?.TryTypeCast<EntityConfiguration<T>>();
        var serializer = typedConfiguration?.GetSerializer();

        return serializer;
    }

}