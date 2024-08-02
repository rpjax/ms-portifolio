using LiteDB;
using Aidan.Core;
using Aidan.Core.Linq;
using Aidan.Core.Linq.Expressions;
using Aidan.Core.Patterns;
using System.Collections;
using System.Linq.Expressions;

namespace Aidan.LiteDb.Repositories;

public interface ILiteDbEntity : IEntity
{
    public ObjectId Id { get; set; }
}

[Obsolete("WIP")]
public class LiteDbRepository<TEntity> : IRepository<TEntity> where TEntity : ILiteDbEntity
{
    private LiteDatabase Database { get; }
    private ILiteCollection<TEntity> Collection { get; }

    public LiteDbRepository(LiteDatabase database)
    {
        Database = database;
        Collection = database.GetCollection<TEntity>();
    }

    public void Dispose()
    {
        Database.Dispose();
    }

    public Task CreateAsync(TEntity entity)
    {
        Collection.Insert(entity);
        return Task.CompletedTask;
    }

    public Task CreateAsync(IEnumerable<TEntity> entities)
    {
        Collection.InsertBulk(entities);
        return Task.CompletedTask;
    }

    public IAsyncQueryable<TEntity> AsQueryable()
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(TEntity entity)
    {
        Collection.Update(entity);
        return Task.CompletedTask;
    }

    public Task<long> UpdateAsync(IUpdateExpression expression)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(TEntity entity)
    {
        Collection.Delete(new BsonValue(entity.Id));
        throw new NotImplementedException();
    }

    public Task<long> DeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        throw new NotImplementedException();
    }

}

[Obsolete("WIP")]
public class LiteDbAsyncQueryable<TEntity> : IAsyncQueryable<TEntity> where TEntity : ILiteDbEntity
{
    public Type ElementType { get; }

    public Expression Expression { get; }

    public IQueryProvider Provider { get; }

    public Task<double> AverageAsync(Expression<Func<TEntity, double>> selector)
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync()
    {
        
        throw new NotImplementedException();
    }

    public IAsyncQueryable<TResult> CreateQuery<TResult>(IQueryable<TResult> source)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> FirstAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> FirstOrDefaultAsync()
    {
        throw new NotImplementedException();
    }

    public IEnumerator<TEntity> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public Task<long> LongCountAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> MaxAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> MinAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> SingleAsync()
    {
        throw new NotImplementedException();
    }

    public Task<TEntity?> SingleOrDefaultAsync()
    {
        throw new NotImplementedException();
    }

    public Task<decimal> SumAsync(Expression<Func<TEntity, decimal>> selector)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity[]> ToArrayAsync()
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> ToListAsync()
    {
        throw new NotImplementedException();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        throw new NotImplementedException();
    }
}
