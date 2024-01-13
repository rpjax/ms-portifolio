using ModularSystem.Core.Expressions;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Provides a set of hooks for intercepting and extending the behavior of CRUD operations on entities of type <typeparamref name="T"/>. <br/>
/// Acts as a foundational layer for concrete middlewares, facilitating the incorporation of custom logic during entity operations.
/// </summary>
/// <typeparam name="T">The type of the entity extending <see cref="IQueryableModel"/>.</typeparam>
public abstract class EntityMiddleware<T> where T : IQueryableModel
{
    /// <summary>
    /// Provides a middleware interception point for custom processing or modification of the provided <paramref name="queryable"/> during the creation of a new queryable instance in the `CreateQueryAsync` method of the `EntityService`.
    /// In this default implementation, the <paramref name="queryable"/> is returned unchanged.
    /// Subclasses or implementations can override this method to incorporate specific middleware behaviors or transformations on the <paramref name="queryable"/>.
    /// </summary>
    /// <param name="queryable">The initial queryable derived from the data layer implementation.</param>
    /// <returns>The modified or original <see cref="IQueryable{T}"/> ready for returning to the caller.</returns>
    public virtual Task<IQueryable<T>> OnCreateQueryAsync(IQueryable<T> queryable) => Task.FromResult(queryable);

    /// <summary>
    /// Intercepting hook executed before the validation of an entity.
    /// </summary>
    /// <param name="entity">The entity about to be validated.</param>
    /// <returns>The potentially modified entity to proceed with validation.</returns>
    public virtual Task<T> BeforeValidateAsync(T entity) => Task.FromResult(entity);

    /// <summary>
    /// Intercepting hook executed before the creation of an entity.
    /// </summary>
    /// <param name="entity">The entity about to be created.</param>
    /// <returns>The potentially modified entity to proceed with creation.</returns>
    public virtual Task<T> BeforeCreateAsync(T entity) => Task.FromResult(entity);

    /// <summary>
    /// Post-process hook executed after the creation of an entity.
    /// </summary>
    /// <param name="entity">The entity that was created.</param>
    /// <returns>Potentially post-processed entity after creation.</returns>
    public virtual Task<T> AfterCreateAsync(T entity) => Task.FromResult(entity);

    /// <summary>
    /// Intercepting hook executed before creating multiple entities.
    /// </summary>
    /// <param name="entities">The entities about to be created.</param>
    /// <returns>The potentially modified list of entities to proceed with creation.</returns>
    public virtual async Task<IEnumerable<T>> BeforeCreateAsync(IEnumerable<T> entities)
    {
        foreach (var item in entities)
        {
            await BeforeCreateAsync(item);
        }

        return entities;
    }

    /// <summary>
    /// Post-process hook executed after creating multiple entities.
    /// </summary>
    /// <param name="entities">The entities that were created.</param>
    /// <returns>Potentially post-processed list of entities after creation.</returns>
    public virtual async Task<IEnumerable<T>> AfterCreateAsync(IEnumerable<T> entities)
    {
        foreach (var item in entities)
        {
            await AfterCreateAsync(item);
        }

        return entities;
    }

    /// <summary>
    /// Intercepting hook executed before a query operation.
    /// </summary>
    /// <param name="query">The query parameters being executed.</param>
    public virtual Task<IQuery<T>> BeforeQueryAsync(IQuery<T> query) => Task.FromResult(query);

    /// <summary>
    /// Post-process hook executed after a query operation.
    /// </summary>
    /// <param name="queryResult">The results of the executed query.</param>
    /// <returns>Potentially post-processed query results.</returns>
    public virtual Task<IQueryResult<T>> AfterQueryAsync(IQueryResult<T> queryResult) => Task.FromResult(queryResult);

    /// <summary>
    /// Intercepting hook executed before an entity update.
    /// </summary>
    /// <param name="currentEntity">The current state of the entity before the update.</param>
    /// <param name="updatedEntity">The new state the entity is proposed to transition to.</param>
    public virtual Task<(T, T)> BeforeUpdateAsync(T currentEntity, T updatedEntity) => Task.FromResult((currentEntity, updatedEntity));

    /// <summary>
    /// Post-process hook executed after an entity update.
    /// </summary>
    /// <param name="currentEntity">The state of the entity before the update.</param>
    /// <param name="updatedEntity">The updated state of the entity.</param>
    /// <returns>A tuple containing the potentially post-processed entities after update.</returns>
    public virtual Task<(T, T)> AfterUpdateAsync(T currentEntity, T updatedEntity) => Task.FromResult((currentEntity, updatedEntity));

    /// <summary>
    /// Intercepting hook executed before an update operation.
    /// </summary>
    /// <param name="update">The update parameters being executed.</param>
    /// <returns>The potentially modified update parameters to proceed with the update operation.</returns>
    public virtual Task<IUpdate<T>> BeforeUpdateAsync(IUpdate<T> update) => Task.FromResult(update);

    /// <summary>
    /// Post-process hook executed after an update operation.
    /// </summary>
    /// <param name="update">The update parameters that were executed.</param>
    /// <returns>Potentially post-processed update parameters after the update operation.</returns>
    public virtual Task<IUpdate<T>> AfterUpdateAsync(IUpdate<T> update) => Task.FromResult(update);

    /// <summary>
    /// Intercepting hook executed before the deletion of an entity.
    /// </summary>
    /// <param name="predicate">The condition used to identify the entity to be deleted.</param>
    public virtual Task<Expression<Func<T, bool>>> BeforeDeleteAsync(Expression<Func<T, bool>> predicate) => Task.FromResult(predicate);

    /// <summary>
    /// Post-process hook executed after the deletion of an entity.
    /// </summary>
    /// <param name="predicate">The condition used to identify the entity that was deleted.</param>
    /// <returns>Potentially post-processed condition after the deletion.</returns>
    public virtual Task<Expression<Func<T, bool>>> AfterDeleteAsync(Expression<Func<T, bool>> predicate) => Task.FromResult(predicate);

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
    public virtual Task<Expression<Func<T, bool>>> BeforeCountAsync(Expression<Func<T, bool>> predicate) => Task.FromResult(predicate);

    /// <summary>
    /// Post-process hook executed after counting entities matching a specified condition.
    /// </summary>
    /// <param name="predicate">The condition that was used for counting.</param>
    /// <returns>Potentially post-processed condition after counting.</returns>
    public virtual Task<Expression<Func<T, bool>>> AfterCountAsync(Expression<Func<T, bool>> predicate) => Task.FromResult(predicate);

    /// <summary>
    /// Intercepting hook executed before counting all entities of type <typeparamref name="T"/>.
    /// </summary>
    public virtual Task BeforeCountAllAsync() => Task.CompletedTask;

    /// <summary>
    /// Post-process hook executed after counting all entities of type <typeparamref name="T"/>.
    /// </summary>
    public virtual Task AfterCountAllAsync() => Task.CompletedTask;
}

/// <summary>
/// Serves as a foundational visitor for query-related expressions and constructs for entities of type <typeparamref name="T"/>. It facilitates inspecting and potentially transforming these elements, offering a blueprint for customization in derived classes.
/// </summary>
/// <typeparam name="T">The type of entity under query operations.</typeparam>
public class EntityExpressionVisitor<T> where T : IQueryableModel
{
    /// <summary>
    /// Offers a foundational way to inspect an expression without making alterations. <br/>
    /// Concrete implementations should implement their logic here.
    /// </summary>
    /// <param name="expression">Expression to inspect.</param>
    /// <returns>The original expression.</returns>
    public virtual Task<Expression> VisitExpressionAsync(Expression expression)
    {
        return Task.FromResult(expression);
    }

    /// <summary>
    /// Visits and potentially modifies various parts of an IQuery object for the entity type.
    /// </summary>
    /// <param name="query">The query to be visited and potentially modified.</param>
    /// <returns>The modified or unmodified query.</returns>
    public virtual async Task<IQuery<T>> VisitQueryAsync(IQuery<T> query)
    {
        var tasks = new List<Task>();

        if (query.Filter != null)
        {
            tasks.Add(Task.Run(async () => query.Filter = await VisitQueryFilter(query.Filter)));
        }

        if (query.Ordering != null)
        {
            tasks.Add(Task.Run(async () => query.Ordering = await VisitQueryOrdering(query.Ordering)));
        }

        tasks.Add(Task.Run(async () => query.Pagination = await VisitQueryPaginationAsync(query.Pagination)));

        await Task.WhenAll(tasks);

        return query;
    }

    /// <summary>
    /// Visits a query's filter expression.
    /// </summary>
    /// <param name="expression">The filter expression to be visited.</param>
    /// <returns>The original or a potentially modified expression.</returns>
    public virtual Task<Expression> VisitQueryFilter(Expression expression)
    {
        return VisitExpressionAsync(expression);
    }

    /// <summary>
    /// Visits a query's order-by expression.
    /// </summary>
    /// <param name="expression">The order expression to be visited.</param>
    /// <returns>The original or a potentially modified expression.</returns>
    public virtual Task<Expression> VisitQueryOrdering(Expression expression)
    {
        return VisitExpressionAsync(expression);
    }

    /// <summary>
    /// Visits a query's pagination information.
    /// </summary>
    /// <param name="pagination">The pagination details to be visited.</param>
    /// <returns>The original or a potentially modified pagination.</returns>
    public virtual Task<PaginationIn> VisitQueryPaginationAsync(PaginationIn pagination)
    {
        return Task.FromResult(pagination);
    }

    /// <summary>
    /// Inspects the provided update operation's filter and modification expressions. Offers a foundational way to analyze without alterations. <br/>
    /// Concrete implementations can provide their transformation logic if needed.
    /// </summary>
    /// <param name="update">The update operation containing selector and modification expressions.</param>
    /// <returns>Potentially transformed update operation.</returns>
    public virtual async Task<IUpdate<T>> VisitUpdateAsync(IUpdate<T> update)
    {
        var reader = new UpdateReader<T>(update);
        var filter = reader.GetFilterExpression();

        if (filter != null)
        {
            update.Filter = await VisitExpressionAsync(filter);
        }

        if (update.Modifications == null)
        {
            return update;
        }

        for (int i = 0; i < update.Modifications.Count; i++)
        {
            update.Modifications[i] = await VisitExpressionAsync(update.Modifications[i]);
        }

        return update;
    }
}

/// <summary>
/// A middleware converter that employs an EntityVisitor to inspect and transform
/// query-related constructs before they are processed by the main query system.
/// </summary>
/// <typeparam name="T">The type of entity being queried.</typeparam>
internal class VisitorMiddlewareConverter<T> : EntityMiddleware<T> where T : IQueryableModel
{
    /// <summary>
    /// The EntityVisitor used for inspecting and transforming query-related constructs.
    /// </summary>
    private EntityExpressionVisitor<T> Visitor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="VisitorMiddlewareConverter{T}"/> class.
    /// </summary>
    /// <param name="visitor">The EntityVisitor to be used.</param>
    public VisitorMiddlewareConverter(EntityExpressionVisitor<T> visitor)
    {
        Visitor = visitor;
    }

    public override Task<IQuery<T>> BeforeQueryAsync(IQuery<T> query)
    {
        return Visitor.VisitQueryAsync(query);
    }

    public override Task<IUpdate<T>> BeforeUpdateAsync(IUpdate<T> update)
    {
        return Visitor.VisitUpdateAsync(update);
    }

    public override async Task<Expression<Func<T, bool>>> BeforeDeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return (await Visitor.VisitExpressionAsync(predicate)).TypeCast<Expression<Func<T, bool>>>();
    }

    public override async Task<Expression<Func<T, bool>>> BeforeCountAsync(Expression<Func<T, bool>> predicate)
    {
        return (await Visitor.VisitExpressionAsync(predicate)).TypeCast<Expression<Func<T, bool>>>();
    }
}

/// <summary>
/// Provides mechanisms for normalizing LINQ expressions pertaining to entities of type T. <br/>
/// Especially useful for ensuring that query expressions are in a standard form before processing.
/// </summary>
/// <typeparam name="T">The type of entity being queried.</typeparam>
internal class ExpressionNormalizer<T> : EntityExpressionVisitor<T> where T : IQueryableModel
{
    /// <summary>
    /// A function to create an ID selector from a parameter expression.
    /// </summary>
    public Func<ParameterExpression, Expression> CreateIdSelectorFunction { get; }

    /// <summary>
    /// A function to parse a string into an object representing an entity ID.
    /// </summary>
    public Func<string, object> ParseIdFunction { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExpressionNormalizer{T}"/> class.
    /// </summary>
    /// <param name="createIdSelectorFunction">The function to create an ID selector.</param>
    /// <param name="parseIdFunction">The function to parse an ID.</param>
    public ExpressionNormalizer(Func<ParameterExpression, Expression> createIdSelectorFunction, Func<string, object> parseIdFunction)
    {
        CreateIdSelectorFunction = createIdSelectorFunction;
        ParseIdFunction = parseIdFunction;
    }

    public override Task<Expression> VisitExpressionAsync(Expression expression)
    {
        expression = new EntityLinqNormalizerVisitor<T>(CreateIdSelectorFunction, ParseIdFunction).
            Visit(expression);

        return base.VisitExpressionAsync(expression);
    }

}
