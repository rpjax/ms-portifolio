using ModularSystem.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace ModularSystem.Mongo;

public class MongoDataAccessObject<T> : IDataAccessObject<T> where T : IMongoModel
{
    protected IMongoCollection<T> collection;
    protected FilterDefinition<T> baseFilter => EmptyFilter();
    protected FilterDefinitionBuilder<T> filterBuilder => MongoModule.GetFilterBuilder<T>();
    protected UpdateDefinitionBuilder<T> updateBuilder => MongoModule.GetUpdateBuilder<T>();

    public MongoDataAccessObject(IMongoCollection<T> collection)
    {
        this.collection = collection;
    }

    //*
    // Static methods
    //*
    public static FilterDefinition<T> EmptyFilter()
    {
        return MongoModule.GetFilterBuilder<T>().Empty;
    }

    public static FilterDefinition<T> IdFilter(string value)
    {
        var id = ObjectId.Parse(value);
        return MongoModule.GetFilterBuilder<T>().Where(x => x.Id == id);
    }

    //*
    // Instance methods
    //*
    public virtual void Dispose()
    {

    }

    //*
    // CREATE
    //*
    public virtual async Task<string> InsertAsync(T data)
    {
        await collection.InsertOneAsync(data);
        return data.GetId();
    }

    public virtual Task InsertAsync(IEnumerable<T> data)
    {
        return collection.InsertManyAsync(data);
    }

    //*
    // READ.
    //*
    public virtual async Task<T> GetAsync(string id)
    {
        var asyncCursor = await collection.FindAsync(IdFilter(id));
        return asyncCursor.First();
    }

    public virtual Task<IQueryResult<T>> QueryAsync(IQuery<T> search)
    {
        var serachObject = new MongoSearch<T>(MongoSearchAsync);
        return serachObject.RunAsync(search);
    }

    public virtual async Task<IQueryResult<T>> MongoSearchAsync(FilterDefinition<T> filter, PaginationIn pagination, SortDefinition<T>? sort = null, ProjectionDefinition<T>? projection = null)
    {
        var options = new FindOptions<T, T>()
        {
            Skip = pagination.Offset,
            Limit = pagination.Limit,
            Projection = projection ?? Builders<T>.Projection.Combine(),
            Sort = sort
        };

        var query = await collection.FindAsync(filter, options);
        var data = await query.ToListAsync();
        var total = await MongoCountAsync(filter);
        var paginationOut = new PaginationOut() { Total = total, Limit = pagination.Limit, Offset = pagination.Offset };

        return new QueryResult<T>(data, paginationOut); ;
    }

    //*
    // UPDATE.
    //*
    public virtual async Task UpdateAsync(T entry)
    {
        await collection.ReplaceOneAsync(IdFilter(entry.GetId()), entry);
    }

    public Task UpdateAsync<TField>(Expression<Func<T, bool>> selector, Expression<Func<T, TField?>> fieldSelector, TField? value)
    {
        return collection.UpdateManyAsync(filterBuilder.Where(selector), updateBuilder.Set(fieldSelector, value));
    }

    public virtual Task MongoUpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        return collection.UpdateOneAsync(filter, update);
    }

    //*
    // DELETE.
    //*
    public Task DeleteAsync(string id)
    {
        var filter = IdFilter(id);
        return collection.DeleteOneAsync(filter);
    }

    public Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return collection.DeleteManyAsync(predicate);
    }

    public Task DeleteAllAsync()
    {
        return collection.DeleteManyAsync(x => true);
    }

    public Task SoftDeleteAsync(string id)
    {
        var update = updateBuilder.Set(x => x.IsSoftDeleted, true);
        var filter = IdFilter(id);
        return collection.UpdateOneAsync(filter, update);
    }

    //*
    // IQueryable interface.
    //*
    public IQueryable<T> AsQueryable()
    {
        return collection.AsQueryable();
    }

    //*
    // Counting methods.
    //*
    public Task<long> CountAsync(string id)
    {
        var filter = IdFilter(id);
        return collection.CountDocumentsAsync(filter);
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> selector)
    {
        var filter = filterBuilder.Where(selector);
        return collection.CountDocumentsAsync(filter);
    }

    public Task<long> CountAllAsync()
    {
        return collection.CountDocumentsAsync(baseFilter);
    }

    public virtual Task<long> MongoCountAsync(FilterDefinition<T> filter)
    {
        return collection.CountDocumentsAsync(filter);
    }

    //*
    // ID validation methods.
    //*
    public bool ValidateIdFormat(string id)
    {
        return ObjectId.TryParse(id, out var _);
    }

    public async Task<bool> ValidateIdAsync(string id)
    {
        if (ValidateIdFormat(id))
        {
            var count = await CountAsync(id);
            return count > 0;
        }

        return false;
    }
}