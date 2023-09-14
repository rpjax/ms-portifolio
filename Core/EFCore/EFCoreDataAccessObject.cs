using Microsoft.EntityFrameworkCore;
using ModularSystem.Core;
using System;
using System.Linq.Expressions;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Entity Framework Core DAO.
/// </summary>
public class EFCoreDataAccessObject<T> : IDataAccessObject<T> where T : class, IEFModel
{
    protected readonly DbContext dbContext;
    protected readonly DbSet<T> dbSet;

    public EFCoreDataAccessObject(DbContext dbContext)
    {
        dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        this.dbContext = dbContext;
        dbSet = dbContext.Set<T>();
    }

    public virtual void Dispose()
    {
        dbContext.Dispose();
    }

    public async Task<string> InsertAsync(T data)
    {
        dbSet.Add(data);
        await dbContext.SaveChangesAsync();
        return data.Id.ToString();
    }

    public Task InsertAsync(IEnumerable<T> entries)
    {
        dbSet.AddRange(entries);
        return dbContext.SaveChangesAsync();
    }

    public Task<T> GetAsync(string id)
    {
        var _id = int.Parse(id);
        return AsQueryable().Where(x => x.Id == _id).FirstAsync();
    }

    public Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        return new EFCoreQueryOperation<T>(dbSet).ExecuteAsync(query);
    }

    public async Task UpdateAsync(T data)
    {
        dbSet.Attach(data);
        dbContext.Entry(data).State = EntityState.Modified;
        await dbContext.SaveChangesAsync();
    }

    public Task UpdateAsync<TField>(Expression<Func<T, bool>> selector, Expression<Func<T, TField?>> fieldSelector, TField? value)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(string id)
    {
        var _id = ParseId(id);
        return dbSet.Where(x => x.Id == _id).ExecuteDeleteAsync();
    }

    public Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return dbSet.Where(predicate).ExecuteDeleteAsync();
    }

    public Task DeleteAllAsync()
    {
        return dbSet.ExecuteDeleteAsync();
    }

    public IQueryable<T> AsQueryable()
    {
        return dbSet.AsQueryable();
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
            return await dbSet.Where(x => x.Id == _id).AnyAsync();
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

public class EFCoreQueryOperation<T> where T : class, IEFModel
{
    readonly DbSet<T> dbSet;

    public EFCoreQueryOperation(DbSet<T> dbSet)
    {
        this.dbSet = dbSet;
    }

    public async Task<IQueryResult<T>> ExecuteAsync(IQuery<T> query)
    {
        var data = await GetData(query);
        var pagination = await GetPagination(query);

        return new QueryResult<T>(data, pagination);
    }

    Task<T[]> GetData(IQuery<T> query)
    {
        var queryable = dbSet.AsQueryable();

        if (query.Filter != null)
        {
            queryable = queryable.Where(query.Filter);
        }

        if (query.Sort != null)
        {
            if (query.Order == Ordering.Descending)
            {
                queryable = queryable.OrderByDescending(query.Sort);
            }
            else
            {
                queryable = queryable.OrderBy(query.Sort);
            }
        }
        else
        {
            queryable = queryable.OrderBy(x => x);
        }

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

        queryable = queryable
            .Skip(query.Pagination.Offset)
            .Take(query.Pagination.Limit);

        return queryable.ToArrayAsync();
    }

    Task<long> GetTotal(IQuery<T> query)
    {
        var queryable = dbSet.AsQueryable();

        if (query.Filter != null)
        {
            queryable = queryable.Where(query.Filter);
        }

        return queryable.LongCountAsync();
    }

    async Task<PaginationOut> GetPagination(IQuery<T> query)
    {
        var total = await GetTotal(query);

        return new PaginationOut()
        {
            Limit = query.Pagination.Limit,
            Offset = query.Pagination.Offset,
            Total = total
        };
    }
}