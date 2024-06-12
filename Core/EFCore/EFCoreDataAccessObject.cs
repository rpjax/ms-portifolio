using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using ModularSystem.Core.Linq;
using ModularSystem.EntityFramework.Linq;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Entity Framework Core DAO.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public class EFCoreDataAccessObject<T> : IDataAccessObject<T> where T : class, IEFEntity
{
    /// <summary>
    /// The DbContext used for database operations.
    /// </summary>
    protected DbContext DbContext { get; }

    /// <summary>
    /// The DbSet representing the entity in the DbContext.
    /// </summary>
    protected DbSet<T> DbSet { get; }

    /// <summary>
    /// Represents the configuration settings used for database operations in this entity access object.
    /// </summary>
    protected Configuration Config { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EFCoreDataAccessObject{T}"/> class.
    /// </summary>
    /// <param name="dbContext">The DbContext used for database operations.</param>
    /// <param name="config">The configuration settings used for database operations.</param>
    public EFCoreDataAccessObject(DbContext dbContext, Configuration? config = null)
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
        Config = config ?? new();
    }

    /// <summary>
    /// Disposes the DbContext.
    /// </summary>
    public virtual void Dispose()
    {
        DbContext.Dispose();
    }

    /// <summary>
    /// Inserts a new entity asynchronously.
    /// </summary>
    /// <param name="data">The entity to insert.</param>
    /// <returns>The ID of the inserted entity.</returns>
    public async Task<string> InsertAsync(T data)
    {
        DbSet.Add(data);
        await DbContext.SaveChangesAsync();
        return data.Id.ToString();
    }

    /// <summary>
    /// Inserts multiple entities asynchronously.
    /// </summary>
    /// <param name="entries">The entities to insert.</param>
    public Task InsertAsync(IEnumerable<T> entries)
    {
        DbSet.AddRange(entries);
        return DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>The retrieved entity.</returns>
    public Task<T> GetAsync(string id)
    {
        var _id = int.Parse(id);
        return AsAsyncQueryable().Where(x => x.Id == _id).FirstAsync();
    }

    /// <summary>
    /// Executes a query asynchronously.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <returns>The result of the query.</returns>
    public Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        return new EFCoreQueryOperation<T>(DbSet).ExecuteAsync(query);
    }

    /// <summary>
    /// Updates an entity asynchronously.
    /// </summary>
    /// <param name="data">The entity to update.</param>
    public async Task UpdateAsync(T data)
    {
        DbSet.Attach(data);
        DbContext.Entry(data).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
    }

    /// <summary>
    /// Updates entities based on a specified update definition asynchronously.
    /// </summary>
    /// <param name="update">The update definition.</param>
    /// <returns>The number of entities updated.</returns>
    public virtual async Task<long?> UpdateAsync(IUpdate<T> update)
    {
        return await new EFCoreUpdateOperation<T>(DbSet, Config.AllowUpdatesWithNoFilter).ExecuteAsync(update);
    }

    /// <summary>
    /// Updates a specific field of entities matching a selector asynchronously.
    /// </summary>
    /// <typeparam name="TField">The type of the field to update.</typeparam>
    /// <param name="selector">The selector for entities to update.</param>
    /// <param name="fieldSelector">The selector for the field to update.</param>
    /// <param name="value">The new value for the field.</param>
    public Task UpdateAsync<TField>(Expression<Func<T, bool>> selector, Expression<Func<T, TField?>> fieldSelector, TField? value)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Deletes an entity by its ID asynchronously.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    public Task DeleteAsync(string id)
    {
        var _id = ParseId(id);
        return DbSet.Where(x => x.Id == _id).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Deletes entities based on a specified predicate asynchronously.
    /// </summary>
    /// <param name="predicate">The predicate to filter entities to delete.</param>
    /// <returns>The number of entities deleted.</returns>
    public async Task<long?> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.Where(predicate).ExecuteDeleteAsync();
    }

    /// <summary>
    /// Deletes all entities asynchronously.
    /// </summary>
    public Task DeleteAllAsync()
    {
        return DbSet.ExecuteDeleteAsync();
    }

    /// <summary>
    /// Returns an IQueryable representing the entities.
    /// </summary>
    /// <returns>An IQueryable representing the entities.</returns>
    public IQueryable<T> AsQueryable()
    {
        return DbSet.AsQueryable();
    }

    /// <summary>
    /// Returns an IAsyncQueryable representing the entities.
    /// </summary>
    /// <returns>An IAsyncQueryable representing the entities.</returns>
    public IAsyncQueryable<T> AsAsyncQueryable()
    {
        return new EFCoreAsyncQueryable<T>(DbSet.AsQueryable());
    }

    /// <summary>
    /// Counts the number of entities with a specific ID asynchronously.
    /// </summary>
    /// <param name="id">The ID to count.</param>
    /// <returns>The number of entities with the specified ID.</returns>
    public Task<long> CountAsync(string id)
    {
        var _id = ParseId(id);
        return CountAsync(x => x.Id == _id);
    }

    /// <summary>
    /// Counts the number of entities that satisfy a specified predicate asynchronously.
    /// </summary>
    /// <param name="selector">The predicate to filter entities.</param>
    /// <returns>The number of entities that satisfy the predicate.</returns>
    public Task<long> CountAsync(Expression<Func<T, bool>> selector)
    {
        return AsAsyncQueryable().LongCountAsync(selector);
    }

    /// <summary>
    /// Counts the total number of entities asynchronously.
    /// </summary>
    /// <returns>The total number of entities.</returns>
    public Task<long> CountAllAsync()
    {
        return AsAsyncQueryable().LongCountAsync();
    }

    /// <summary>
    /// Validates the format of an ID string.
    /// </summary>
    /// <param name="id">The ID string to validate.</param>
    /// <returns><c>true</c> if the ID format is valid; otherwise, <c>false</c>.</returns>
    public bool ValidateIdFormat(string id)
    {
        try
        {
            return ParseId(id) > 0;
        }
        catch (Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Validates the format of an ID string asynchronously.
    /// </summary>
    /// <param name="id">The ID string to validate.</param>
    /// <returns><c>true</c> if the ID format is valid and the entity exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> ValidateIdAsync(string id)
    {
        if (ValidateIdFormat(id))
        {
            var _id = ParseId(id);
            return await DbSet.Where(x => x.Id == _id).AnyAsync();
        }

        return false;
    }

    private long ParseId(string id)
    {
        return long.Parse(id);
    }

    private string GetTableName<TEntity>(DbContext context) where TEntity : class
    {
        var entityType = context.Model.FindEntityType(typeof(TEntity));
        var tableName = entityType.GetTableName();
        var schema = entityType.GetSchema();

        return schema != null ? $"{schema}.{tableName}" : tableName;
    }

    /// <summary>
    /// Represents the configuration settings for database operations.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets or sets a value indicating whether update operations can be performed on all records simultaneously.
        /// When set to <c>true</c>, updates can be executed without specifying a filter definition.
        /// Default value is <c>false</c>.
        /// </summary>
        public bool AllowUpdatesWithNoFilter { get; set; } = false;
    }
}


internal class EFCoreQueryOperation<T> where T : class, IEFEntity
{
    private DbSet<T> DbSet { get; }

    public EFCoreQueryOperation(DbSet<T> dbSet)
    {
        DbSet = dbSet;
    }

    public async Task<IQueryResult<T>> ExecuteAsync(IQuery<T> query)
    {
        var data = await GetDataAsync(query);
        var pagination = await GetPaginationAsync(query);

        return new QueryResult<T>(data);
    }

    Task<T[]> GetDataAsync(IQuery<T> query)
    {
        var builder = new QueryableBuilder<T>(DbSet.AsQueryable(), query);
        var queryable = builder
            .UseFilter()
            .UseOrdering()
            .UsePagination()
            .Create();

        return queryable.ToArrayAsync();
    }

    async Task<PaginationOut> GetPaginationAsync(IQuery<T> query)
    {
        var builder = new QueryableBuilder<T>(DbSet.AsQueryable(), query);
        var queryable = builder
            .UseFilter()
            .Create();

        var total = await queryable.LongCountAsync();

        return new PaginationOut()
        {
            Limit = query.Pagination.Limit,
            Offset = query.Pagination.Offset,
            Total = total
        };
    }

}

internal class QueryableBuilder<T>
{
    IQueryable<T> Queryable { get; set; }
    QueryReader<T> QueryReader { get; init; }

    public QueryableBuilder(IQueryable<T> queryable, IQuery<T> query)
    {
        Queryable = queryable;
        QueryReader = new(query);
    }

    public IQueryable<T> Create()
    {
        return Queryable;
    }

    public QueryableBuilder<T> UseFilter()
    {
        var predicate = QueryReader.GetFilterExpression();

        if (predicate != null)
        {
            Queryable = Queryable.Where(predicate);
        }

        return this;
    }

    public QueryableBuilder<T> UseGrouping()
    {
        // todo...
        return this;
    }

    public QueryableBuilder<T> UseProjection()
    {
        // todo...

        //*
        // NOTE: This code is commented because when the last code review was made, projection was not developed because of a problem in the Expression parser.
        //*

        //if (query.Projection != null)
        //{
        //    //var type = typeof(T);

        //    //if (type.IsAssignableFrom(typeof(ProductJoin)))
        //    //{
        //    //    Console.WriteLine();
        //    //}

        //    queryable = queryable.Select(query.Projection);
        //}
        return this;
    }

    public QueryableBuilder<T> UseOrdering()
    {
        var complexOrdering = QueryReader.GetOrderingExpression();
        var reader = new ComplexOrderingReader<T>(complexOrdering);

        if (complexOrdering != null)
        {
            foreach (var orderingExpression in reader.GetOrderingExpressions())
            {
                MethodInfo? methodInfo;

                var methodName =
                    orderingExpression.Direction == OrderingDirection.Descending
                    ? "OrderByDescending"
                    : "OrderBy";

                methodInfo = typeof(IOrderedEnumerable<T>)
                        .GetMethod(methodName)?
                        .MakeGenericMethod(orderingExpression.FieldType);

                if (methodInfo == null)
                {
                    throw new InvalidOperationException();
                }

                var modifiedQueryable = methodInfo
                    .Invoke(Queryable, new object[] { orderingExpression.FieldSelector })
                    ?.TryTypeCast<IQueryable<T>>();

                if (modifiedQueryable == null)
                {
                    throw new InvalidOperationException();
                }

                Queryable = modifiedQueryable;
            }
        }
        else
        {
            Queryable = Queryable.OrderBy(x => x);
        }

        return this;
    }

    public QueryableBuilder<T> UsePagination()
    {
        Queryable = Queryable
            .Skip(QueryReader.GetIntOffset())
            .Take(QueryReader.GetIntLimit());
        return this;
    }
}

internal class EFCoreUpdateOperation<T> where T : class, IEFEntity
{
    private DbSet<T> DbSet { get; }
    private bool AllowUpdatesWithNoFilter { get; }

    public EFCoreUpdateOperation(DbSet<T> dbSet, bool allowUpdatesWithNoFilter)
    {
        DbSet = dbSet;
        AllowUpdatesWithNoFilter = allowUpdatesWithNoFilter;
    }

    public async Task<long> ExecuteAsync(IUpdate<T> update)
    {
        var reader = new UpdateReader<T>(update);
        var modifications = reader.GetUpdateSetExpressions();

        if (modifications == null || modifications.IsEmpty())
        {
            return 0;
        }

        var queryable = DbSet.AsQueryable();
        var filterExpr = reader.GetFilterExpression();

        if (filterExpr == null)
        {
            if (!AllowUpdatesWithNoFilter)
            {
                throw new Exception("Update operation requires a filter definition. To allow updates without a filter, adjust the settings in the provided configuration object when initializing the entity access object.");
            }
        }
        else
        {
            queryable = queryable.Where(filterExpr);
        }

        //*
        // Developer Notes:
        // The update function takes an expression argument of type:
        // Expression<Func<SetPropertyCalls<TSource>, SetPropertyCalls<TSource>>>
        // 
        // Where the updates are defined as:
        // (set) => set.SetProperty<T>(Func<T, TField> selector, T value)
        //
        // The 'set' object is primarily a LINQ placeholder and shouldn't be used directly.
        // This class is utilized internally by the Entity Framework to interpret updates.
        // This allows for flexible manipulation of the expression.
        //*
        var parameterExpression = Expression.Parameter(typeof(SetPropertyCalls<T>), "s");
        var fluentParameter = parameterExpression as Expression;
        var setCalls = new List<MethodCallExpression>();

        foreach (var updateSet in modifications)
        {
            //*
            // The LINQ method used to generate the update is:
            // new SetPropertyCalls<T>().SetProperty();
            //
            // NOTE: The method is the second element in the sequence produced by:
            // typeof(SetPropertyCalls<T>).GetMethods()
            //
            // If Entity Framework's API change this could require adaptation.
            //*
            var setMethodInfo = typeof(SetPropertyCalls<T>)
                .GetMethods()
                .Where(x => x.Name == "SetProperty")
                .ElementAt(1)
                .MakeGenericMethod(updateSet.FieldType);

            if (setMethodInfo == null)
            {
                throw new InvalidOperationException();
            }

            var selectorFuncDelegate = typeof(Func<,>)
                .MakeGenericType(new Type[] { typeof(T), updateSet.FieldType });
            var selectorExprDelegate = typeof(Expression<>)
                .MakeGenericType(selectorFuncDelegate);

            var selectorExpr = updateSet.ToLambda(Expression.Parameter(typeof(T), "x"));

            if (selectorExpr == null)
            {
                throw new Exception("Invalid or insupported UpdateSetExpression expression.");
            }

            var setMethodArgs = new Expression[]
            {
                selectorExpr,
                updateSet.Value
            };

            fluentParameter = Expression.Call(fluentParameter, setMethodInfo, setMethodArgs);
        }

        var lambdaExpression = Expression
            .Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(fluentParameter, parameterExpression);

        var visitor = new ParameterExpressionReferenceBinder();

        lambdaExpression = visitor.VisitAndConvert(lambdaExpression, "VisitLambda");

        return await queryable.ExecuteUpdateAsync(lambdaExpression);
    }

}