using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using MongoDB.Driver;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Entity Framework Core DAO.
/// </summary>
public class EFCoreDataAccessObject<T> : IDataAccessObject<T> where T : class, IEFModel
{
    protected DbContext DbContext { get; }
    protected DbSet<T> DbSet { get; }

    /// <summary>
    /// Represents the configuration settings used for database operations in this data access object.
    /// </summary>
    protected Configuration Config { get; }

    public EFCoreDataAccessObject(DbContext dbContext, Configuration? config = null)
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
        Config = config ?? new();
    }

    public virtual void Dispose()
    {
        DbContext.Dispose();
    }

    public async Task<string> InsertAsync(T data)
    {
        DbSet.Add(data);
        await DbContext.SaveChangesAsync();
        return data.Id.ToString();
    }

    public Task InsertAsync(IEnumerable<T> entries)
    {
        DbSet.AddRange(entries);
        return DbContext.SaveChangesAsync();
    }

    public Task<T> GetAsync(string id)
    {
        var _id = int.Parse(id);
        return AsQueryable().Where(x => x.Id == _id).FirstAsync();
    }

    public Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        return new EFCoreQueryOperation<T>(DbSet).ExecuteAsync(query);
    }

    public async Task UpdateAsync(T data)
    {
        DbSet.Attach(data);
        DbContext.Entry(data).State = EntityState.Modified;
        await DbContext.SaveChangesAsync();
    }

    public virtual Task<long?> UpdateAsync(IUpdate<T> update)
    {
        return new EFCoreUpdateOperation<T>(DbSet, Config).ExecuteAsync(update);
    }

    public Task UpdateAsync<TField>(Expression<Func<T, bool>> selector, Expression<Func<T, TField?>> fieldSelector, TField? value)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        var _id = ParseId(id);
        return DbSet.Where(x => x.Id == _id).ExecuteDeleteAsync();
    }

    public async Task<long?> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return await DbSet.Where(predicate).ExecuteDeleteAsync();
    }

    public Task DeleteAllAsync()
    {
        return DbSet.ExecuteDeleteAsync();
    }

    public IQueryable<T> AsQueryable()
    {
        return DbSet.AsQueryable();
    }

    public Task<long> CountAsync(string id)
    {
        var _id = ParseId(id);
        return CountAsync(x => x.Id == _id);
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> selector)
    {
        return AsQueryable().LongCountAsync(selector);
    }

    public Task<long> CountAllAsync()
    {
        return AsQueryable().LongCountAsync();
    }

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

    public async Task<bool> ValidateIdAsync(string id)
    {
        if (ValidateIdFormat(id))
        {
            var _id = ParseId(id);
            return await DbSet.Where(x => x.Id == _id).AnyAsync();
        }

        return false;
    }

    long ParseId(string id)
    {
        return long.Parse(id);
    }

    string GetTableName<TEntity>(DbContext context) where TEntity : class
    {
        // Access the Model for the entity type.
        var entityType = context.Model.FindEntityType(typeof(TEntity));

        // Find the table name.
        var tableName = entityType.GetTableName();

        // Find the schema if needed.
        var schema = entityType.GetSchema();

        return schema != null ? $"{schema}.{tableName}" : tableName;
    }

    /// <summary>
    /// Represents the configuration settings for database operations.
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Gets or sets a value indicating whether update operations can be performed on all records simultaneously.<br/>
        /// When set to <c>true</c>, updates can be executed without specifying a filter definition.<br/>
        /// Default value is <c>false</c>.
        /// </summary>
        /// <value>
        ///   <c>true</c> if updates without a filter are allowed; otherwise, <c>false</c>.
        /// </value>
        public bool AllowUpdatesWithNoFilter { get; set; } = false;
    }

}

internal class EFCoreQueryOperation<T> where T : class, IEFModel
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

        return new QueryResult<T>(data, pagination);
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

internal class EFCoreUpdateOperation<T> where T : class, IEFModel
{
    private DbSet<T> DbSet { get; }
    private EFCoreDataAccessObject<T>.Configuration Config { get; }

    public EFCoreUpdateOperation(DbSet<T> dbSet, EFCoreDataAccessObject<T>.Configuration config)
    {
        DbSet = dbSet;
        Config = config;
    }

    public async Task<long?> ExecuteAsync(IUpdate<T> update)
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
            if (!Config.AllowUpdatesWithNoFilter)
            {
                throw new Exception("Update operation requires a filter definition. To allow updates without a filter, adjust the settings in the provided configuration object when initializing the data access object.");
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

        var visitor = new ParameterExpressionUniformityVisitor();

        lambdaExpression = visitor.VisitAndConvert(lambdaExpression, "VisitLambda");

        return await queryable.ExecuteUpdateAsync(lambdaExpression);
    }

}