using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using ModularSystem.Core;
using System.Linq;
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

    public EFCoreDataAccessObject(DbContext dbContext)
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        DbContext = dbContext;
        DbSet = dbContext.Set<T>();
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

    public virtual Task UpdateAsync(IUpdate<T> update)
    {
        return new EFCoreUpdateOperation<T>(DbSet).ExecuteAsync(update);
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

    public Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return DbSet.Where(predicate).ExecuteDeleteAsync();
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
        var builder = new QueryableBuilder(DbSet.AsQueryable(), query);
        var queryable = builder
            .UseFilter()
            .UseOrdering()
            .UsePagination()
            .Create();

        return queryable.ToArrayAsync();
    }

    async Task<PaginationOut> GetPaginationAsync(IQuery<T> query)
    {
        var builder = new QueryableBuilder(DbSet.AsQueryable(), query);
        var queryable = builder.UseFilter().Create();
        var total = await queryable.LongCountAsync();

        return new PaginationOut()
        {
            Limit = query.Pagination.Limit,
            Offset = query.Pagination.Offset,
            Total = total
        };
    }

    internal class QueryableBuilder
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

        public QueryableBuilder UseFilter()
        {
            var predicate = QueryReader.GetFilterExpression();

            if (predicate != null)
            {
                Queryable = Queryable.Where(predicate);
            }

            return this;
        }

        public QueryableBuilder UseGrouping()
        {
            // todo...
            return this;
        }

        public QueryableBuilder UseProjection()
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

        public QueryableBuilder UseOrdering()
        {
            var ordering = QueryReader.GetOrderingExpression();

            if (ordering != null)
            {
                MethodInfo? methodInfo;

                var methodName =
                    QueryReader.GetOrderingDirection() == OrderingDirection.Descending
                    ? "OrderByDescending"
                    : "OrderBy";

                methodInfo = typeof(IOrderedEnumerable<T>)
                        .GetMethod(methodName)?
                        .MakeGenericMethod(ordering.FieldType);

                if (methodInfo == null)
                {
                    throw new InvalidOperationException();
                }

                var modifiedQueryable = methodInfo
                    .Invoke(Queryable, new object[] { ordering.FieldSelector })
                    ?.TryTypeCast<IQueryable<T>>();

                if (modifiedQueryable == null)
                {
                    throw new InvalidOperationException();
                }

                Queryable = modifiedQueryable;
            }
            else
            {
                Queryable = Queryable.OrderBy(x => x);
            }

            return this;
        }

        public QueryableBuilder UsePagination()
        {
            Queryable = Queryable
                .Skip(QueryReader.GetIntOffset())
                .Take(QueryReader.GetIntLimit());
            return this;
        }
    }

}

internal class EFCoreUpdateOperation<T> where T : class, IEFModel
{
    private DbSet<T> DbSet { get; }

    public EFCoreUpdateOperation(DbSet<T> dbSet)
    {
        DbSet = dbSet;
    }

    public async Task ExecuteAsync(IUpdate<T> updateObject)
    {
        if (updateObject is not Update<T>)
        {
            throw new ArgumentNullException();
        }

        var update = updateObject.TypeCast<Update<T>>();
        var modifications = update.ModificationsAsUpdateSets();

        if (modifications == null || modifications.IsEmpty())
        {
            return;
        }

        var reader = new UpdateReader<T>(update);
        var queryable = DbSet.AsQueryable();

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
        //new SetPropertyCalls<T>().SetProperty()
        foreach (var updateSet in modifications)
        {
            var setMethodInfo = typeof(SetPropertyCalls<T>)
                .GetMethod("SetProperty")
                ?.MakeGenericMethod(updateSet.FieldType);

            if (setMethodInfo == null)
            {
                throw new InvalidOperationException();
            }

            var setMethodArgs = new Expression[]
            {
                Expression.Constant(reader.GetFilterExpression(), typeof(Expression<Func<T, bool>>)),
                Expression.Constant(updateSet.Value, updateSet.FieldType)
            };

            var setExpression = Expression.Call(fluentParameter, setMethodInfo, setMethodArgs);

            setCalls.Add(setExpression);
            fluentParameter = setExpression;
        }

        var lambdaExpression = Expression.Lambda<Func<SetPropertyCalls<T>, SetPropertyCalls<T>>>(parameterExpression, parameterExpression);

        var visitor = new ParameterExpressionUniformityVisitor();

        lambdaExpression = visitor.VisitAndConvert(lambdaExpression, "VisitLambda");

        await queryable.ExecuteUpdateAsync(lambdaExpression);
    }

    private Expression<Func<TEntity, TEntity>> CreateUpdateExpression<TEntity>(string propertyName, object newValue)
    {
        var entityParameter = Expression.Parameter(typeof(TEntity), "x");

        var property = Expression.Property(entityParameter, propertyName);
        var value = Expression.Constant(newValue);

        var assign = Expression.Assign(property, value);

        var updateExpression = Expression.Lambda<Func<TEntity, TEntity>>(assign, entityParameter);

        return updateExpression;
    }
}