using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Provides a set of hooks for intercepting and extending the behavior of CRUD operations on entities of type <typeparamref name="T"/>.
/// This abstract class serves as a base for concrete middlewares that may need to implement custom logic around entity operations.
/// </summary>
/// <typeparam name="T">The type of the entity extending <see cref="IQueryableModel"/>.</typeparam>
public abstract class EntityMiddleware<T> where T : IQueryableModel
{
    /// <summary>
    /// Intercepting hook executed before the validation of an entity.
    /// </summary>
    /// <param name="entity">The entity about to be validated.</param>
    /// <returns>The potentially modified entity to proceed with validation.</returns>
    public virtual Task BeforeValidateAsync(T entity) => Task.FromResult(entity);

    /// <summary>
    /// Intercepting hook executed before the creation of an entity.
    /// </summary>
    /// <param name="entity">The entity about to be created.</param>
    /// <returns>The potentially modified entity to proceed with creation.</returns>
    public virtual Task BeforeCreateAsync(T entity) => Task.FromResult(entity);

    /// <summary>
    /// Post-process hook executed after the creation of an entity.
    /// </summary>
    /// <param name="entity">The entity that was created.</param>
    public virtual Task AfterCreateAsync(T entity) => Task.CompletedTask;

    /// <summary>
    /// Intercepting hook executed before a query operation.
    /// </summary>
    /// <param name="query">The query parameters being executed.</param>
    public virtual Task BeforeQueryAsync(IQuery<T> query) => Task.CompletedTask;

    /// <summary>
    /// Post-process hook executed after a query operation.
    /// </summary>
    /// <param name="queryResult">The results of the executed query.</param>
    public virtual Task AfterQueryAsync(IQueryResult<T> queryResult) => Task.CompletedTask;

    /// <summary>
    /// Intercepting hook executed before an entity update.
    /// </summary>
    /// <param name="old">The current state of the entity before the update.</param>
    /// <param name="new">The new state the entity is proposed to transition to.</param>
    public virtual Task BeforeUpdateAsync(T old, T @new) => Task.CompletedTask;

    /// <summary>
    /// Post-process hook executed after an entity update.
    /// </summary>
    /// <param name="old">The state of the entity before the update.</param>
    /// <param name="new">The updated state of the entity.</param>
    public virtual Task AfterUpdateAsync(T old, T @new) => Task.CompletedTask;

    /// <summary>
    /// Intercepting hook executed before the deletion of an entity.
    /// </summary>
    /// <param name="predicate">The condition used to identify the entity to be deleted.</param>
    public virtual Task BeforeDeleteAsync(Expression<Func<T, bool>> predicate) => Task.CompletedTask;

    /// <summary>
    /// Post-process hook executed after the deletion of an entity.
    /// </summary>
    /// <param name="predicate">The condition used to identify the entity that was deleted.</param>
    public virtual Task AfterDeleteAsync(Expression<Func<T, bool>> predicate) => Task.CompletedTask;

    /// <summary>
    /// Intercepting hook executed before the deletion of all entities of type <typeparamref name="T"/>.
    /// </summary>
    public virtual Task BeforeDeleteAllAsync() => Task.CompletedTask;

    /// <summary>
    /// Post-process hook executed after the deletion of all entities of type <typeparamref name="T"/>.
    /// </summary>
    public virtual Task AfterDeleteAllAsync() => Task.CompletedTask;

    /// <summary>
    /// Intercepting hook executed before counting entities matching a specified condition.
    /// </summary>
    /// <param name="predicate">The condition to use for counting.</param>
    public virtual Task BeforeCountAsync(Expression<Func<T, bool>> predicate) => Task.CompletedTask;

    /// <summary>
    /// Post-process hook executed after counting entities matching a specified condition.
    /// </summary>
    /// <param name="predicate">The condition that was used for counting.</param>
    public virtual Task AfterCountAsync(Expression<Func<T, bool>> predicate) => Task.CompletedTask;

    /// <summary>
    /// Intercepting hook executed before counting all entities of type <typeparamref name="T"/>.
    /// </summary>
    public virtual Task BeforeCountAllAsync() => Task.CompletedTask;

    /// <summary>
    /// Post-process hook executed after counting all entities of type <typeparamref name="T"/>.
    /// </summary>
    public virtual Task AfterCountAllAsync() => Task.CompletedTask;
}
