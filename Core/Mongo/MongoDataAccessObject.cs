using ModularSystem.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace ModularSystem.Mongo;

public class MongoDataAccessObject<T> : IDataAccessObject<T> where T : IMongoModel
{
    protected IMongoCollection<T> Collection { get; }
    protected Configuration Config { get; }
    protected FilterDefinition<T> EmptyFilter => MongoModule.GetFilterBuilder<T>().Empty;
    protected FilterDefinitionBuilder<T> FilterBuilder => MongoModule.GetFilterBuilder<T>();
    protected UpdateDefinitionBuilder<T> UpdateBuilder => MongoModule.GetUpdateBuilder<T>();

    public MongoDataAccessObject(IMongoCollection<T> collection, Configuration? config = null)
    {
        Collection = collection;
        Config = config ?? new();
    }

    //*
    // Static methods
    //*

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

    public IQueryable<T> AsQueryable()
    {
        return Collection.AsQueryable();
    }

    //*
    // CREATE
    //*

    public virtual async Task<string> InsertAsync(T data)
    {
        await Collection.InsertOneAsync(data);
        return data.GetId();
    }

    public virtual Task InsertAsync(IEnumerable<T> data)
    {
        return Collection.InsertManyAsync(data);
    }

    //*
    // READ.
    //*

    public virtual async Task<T> GetAsync(string id)
    {
        var asyncCursor = await Collection.FindAsync(IdFilter(id));
        return asyncCursor.First();
    }

    public virtual Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        return new MongoSearch<T>(MongoSearchAsync, query).RunAsync();
    }

    public virtual async Task<IQueryResult<T>> MongoSearchAsync(FilterDefinition<T> filter, PaginationIn pagination, SortDefinition<T>? sort = null, ProjectionDefinition<T>? projection = null)
    {
        var options = new FindOptions<T, T>()
        {
            Skip = Convert.ToInt32(pagination.Offset),
            Limit = Convert.ToInt32(pagination.Limit), 
            Projection = projection ?? Builders<T>.Projection.Combine(),
            Sort = sort
        };

        var query = await Collection.FindAsync(filter, options);
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
        await Collection.ReplaceOneAsync(IdFilter(entry.GetId()), entry);
    }

    public virtual async Task UpdateAsync(IUpdate<T> update)
    {
        var reader = new UpdateReader<T>(update);
        var filter = reader.GetFilterExpression();
        var modifications = reader.GetUpdateSetExpressions().ToArray();

        if(filter == null)
        {
            if (!Config.AllowUpdatesWithNoFilter)
            {
                throw new Exception("Update operation requires a filter definition. To allow updates without a filter, adjust the settings in the provided configuration object when initializing the data access object.");
            }

            filter = x => true;
        }
        if (modifications == null || modifications.IsEmpty())
        {
            return;
        }

        var filterDefinition = FilterBuilder.Where(filter);
        var updateDefinition = UpdateBuilder.Combine();

        foreach (var updateSet in modifications)
        {
            updateDefinition = UpdateBuilder.Combine(updateDefinition, UpdateBuilder.Set(updateSet.FieldName, updateSet.Value));
        }

        await Collection.UpdateManyAsync(filterDefinition, updateDefinition);
    }

    public Task UpdateAsync<TField>(Expression<Func<T, bool>> selector, Expression<Func<T, TField?>> fieldSelector, TField? value)
    {
        return Collection.UpdateManyAsync(FilterBuilder.Where(selector), UpdateBuilder.Set(fieldSelector, value));
    }

    public virtual Task MongoUpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        return Collection.UpdateOneAsync(filter, update);
    }

    //*
    // DELETE.
    //*
    public Task DeleteAsync(string id)
    {
        var filter = IdFilter(id);
        return Collection.DeleteOneAsync(filter);
    }

    public Task DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        return Collection.DeleteManyAsync(predicate);
    }

    public Task DeleteAllAsync()
    {
        return Collection.DeleteManyAsync(x => true);
    }

    public Task SoftDeleteAsync(string id)
    {
        var update = UpdateBuilder.Set(x => x.IsSoftDeleted, true);
        var filter = IdFilter(id);
        return Collection.UpdateOneAsync(filter, update);
    }

    //*
    // Counting methods.
    //*
    public Task<long> CountAsync(string id)
    {
        var filter = IdFilter(id);
        return Collection.CountDocumentsAsync(filter);
    }

    public Task<long> CountAsync(Expression<Func<T, bool>> selector)
    {
        var filter = FilterBuilder.Where(selector);
        return Collection.CountDocumentsAsync(filter);
    }

    public Task<long> CountAllAsync()
    {
        return Collection.CountDocumentsAsync(EmptyFilter);
    }

    public virtual Task<long> MongoCountAsync(FilterDefinition<T> filter)
    {
        return Collection.CountDocumentsAsync(filter);
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