using ModularSystem.Core;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace ModularSystem.Mongo;

/// <summary>
/// Provides a concrete implementation of <see cref="IDataAccessObject{T}"/> tailored for MongoDB operations.<br/>
/// This class encapsulates the CRUD operations and other utility functions specific to MongoDB, ensuring a consistent and efficient data access pattern.
/// </summary>
/// <typeparam name="T">The type of the entity being accessed, which must implement <see cref="IMongoModel"/>.</typeparam>
public class MongoDataAccessObject<T> : IDataAccessObject<T> where T : IMongoModel
{
    /// <summary>
    /// Provides direct access to the MongoDB collection associated with the type <typeparamref name="T"/>.
    /// </summary>
    protected IMongoCollection<T> Collection { get; }

    /// <summary>
    /// Represents the configuration settings used for database operations in this data access object.
    /// </summary>
    protected Configuration Config { get; }

    /// <summary>
    /// Provides an empty filter definition, useful for operations that require a filter but don't need any specific criteria.
    /// </summary>
    protected FilterDefinition<T> EmptyFilter => MongoModule.GetFilterBuilder<T>().Empty;

    /// <summary>
    /// Provides a builder for creating filter definitions for MongoDB operations.
    /// </summary>
    protected FilterDefinitionBuilder<T> FilterBuilder => MongoModule.GetFilterBuilder<T>();

    /// <summary>
    /// Provides a builder for creating update definitions for MongoDB operations.
    /// </summary>
    protected UpdateDefinitionBuilder<T> UpdateBuilder => MongoModule.GetUpdateBuilder<T>();

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoDataAccessObject{T}"/> class.
    /// </summary>
    /// <param name="collection">The MongoDB collection associated with the entity type <typeparamref name="T"/>.</param>
    /// <param name="config">Optional configuration settings for database operations.</param>
    public MongoDataAccessObject(IMongoCollection<T> collection, Configuration? config = null)
    {
        Collection = collection;
        Config = config ?? new();
    }

    /// <summary>
    /// Converts a string ID into a MongoDB filter definition.
    /// </summary>
    /// <param name="value">The string representation of the ID.</param>
    /// <returns>A filter definition targeting the specified ID.</returns>
    public static FilterDefinition<T> IdFilter(string value)
    {
        var id = ObjectId.Parse(value);
        return MongoModule.GetFilterBuilder<T>().Where(x => x.Id == id);
    }

    /// <summary>
    /// Disposes the current instance of the data access object.
    /// </summary>
    public virtual void Dispose()
    {
        // Implementation of the Dispose method.
    }

    /// <summary>
    /// Provides a queryable interface to the MongoDB collection.
    /// </summary>
    /// <returns>An IQueryable interface to the collection.</returns>
    public IQueryable<T> AsQueryable()
    {
        return Collection.AsQueryable();
    }

    /// <summary>
    /// Inserts a single entity into the MongoDB collection.
    /// </summary>
    /// <param name="data">The entity to be inserted.</param>
    /// <returns>The ID of the inserted entity.</returns>
    public virtual async Task<string> InsertAsync(T data)
    {
        await Collection.InsertOneAsync(data);
        return data.GetId();
    }

    /// <summary>
    /// Inserts multiple entities into the MongoDB collection.
    /// </summary>
    /// <param name="data">The entities to be inserted.</param>
    /// <returns>A task representing the asynchronous insert operation.</returns>
    public virtual Task InsertAsync(IEnumerable<T> data)
    {
        return Collection.InsertManyAsync(data);
    }

    /// <summary>
    /// Retrieves a single entity from the MongoDB collection by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <returns>The entity with the specified ID.</returns>
    public virtual async Task<T> GetAsync(string id)
    {
        var asyncCursor = await Collection.FindAsync(IdFilter(id));
        return asyncCursor.First();
    }

    /// <summary>
    /// Executes a query against the MongoDB collection and returns the results.
    /// </summary>
    /// <param name="query">The query to execute.</param>
    /// <returns>The results of the query.</returns>
    public virtual Task<IQueryResult<T>> QueryAsync(IQuery<T> query)
    {
        return new MongoSearch<T>(MongoSearchAsync, query).RunAsync();
    }

    /// <summary>
    /// Executes a MongoDB search operation with the specified parameters.
    /// </summary>
    /// <param name="filter">The filter to apply to the search.</param>
    /// <param name="pagination">Pagination settings for the search.</param>
    /// <param name="sort">Optional sort definition for the search.</param>
    /// <param name="projection">Optional projection definition for the search.</param>
    /// <returns>The results of the search operation.</returns>
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

        return new QueryResult<T>(data, paginationOut);
    }

    /// <summary>
    /// Updates a single entity in the MongoDB collection.
    /// </summary>
    /// <param name="entry">The entity to update.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public virtual async Task UpdateAsync(T entry)
    {
        await Collection.ReplaceOneAsync(IdFilter(entry.GetId()), entry);
    }

    /// <summary>
    /// Updates multiple entities in the MongoDB collection based on the provided update definition.
    /// </summary>
    /// <param name="update">The update definition specifying which entities to update and how to update them.</param>
    /// <returns>
    /// A task that represents the asynchronous update operation. <br/>
    /// The task result contains the number of updated entities 
    /// if the operation was acknowledged and the modified count is available; otherwise, it returns null.
    /// </returns>
    public virtual async Task<long?> UpdateAsync(IUpdate<T> update)
    {
        var reader = new UpdateReader<T>(update);
        var filter = reader.GetFilterExpression();
        var modifications = reader.GetUpdateSetExpressions().ToArray();

        if (filter == null)
        {
            if (!Config.AllowUpdatesWithNoFilter)
            {
                throw new Exception("Update operation requires a filter definition. To allow updates without a filter, adjust the settings in the provided configuration object when initializing the data access object.");
            }

            filter = x => true;
        }
        if (modifications == null || modifications.IsEmpty())
        {
            return 0;
        }

        var filterDefinition = FilterBuilder.Where(filter);
        var updateDefinition = UpdateBuilder.Combine();

        foreach (var updateSet in modifications)
        {
            updateDefinition = UpdateBuilder.Combine(updateDefinition, UpdateBuilder.Set(updateSet.FieldName, updateSet.Value));
        }

        var result = await Collection.UpdateManyAsync(filterDefinition, updateDefinition);

        if (result.IsAcknowledged && result.IsModifiedCountAvailable)
        {
            return result.ModifiedCount;
        }

        return null;
    }

    /// <summary>
    /// Updates entities in the MongoDB collection based on a field selector and a new value.
    /// </summary>
    /// <typeparam name="TField">The type of the field being updated.</typeparam>
    /// <param name="selector">The selector to determine which entities to update.</param>
    /// <param name="fieldSelector">The field selector to specify which field to update.</param>
    /// <param name="value">The new value for the field.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public Task UpdateAsync<TField>(Expression<Func<T, bool>> selector, Expression<Func<T, TField?>> fieldSelector, TField? value)
    {
        return Collection.UpdateManyAsync(FilterBuilder.Where(selector), UpdateBuilder.Set(fieldSelector, value));
    }

    /// <summary>
    /// Executes a MongoDB update operation with the specified filter and update definitions.
    /// </summary>
    /// <param name="filter">The filter to determine which entities to update.</param>
    /// <param name="update">The update definition specifying how to update the entities.</param>
    /// <returns>A task representing the asynchronous update operation.</returns>
    public virtual Task MongoUpdateAsync(FilterDefinition<T> filter, UpdateDefinition<T> update)
    {
        return Collection.UpdateOneAsync(filter, update);
    }

    /// <summary>
    /// Deletes a single entity from the MongoDB collection by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity to delete.</param>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public Task DeleteAsync(string id)
    {
        var filter = IdFilter(id);
        return Collection.DeleteOneAsync(filter);
    }

    /// <summary>
    /// Deletes multiple entities from the MongoDB collection based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to determine which entities to delete.</param>
    /// <returns>
    /// A task that represents the asynchronous delete operation. <br/>
    /// The task result contains the number of deleted entities 
    /// if the operation was acknowledged; otherwise, it returns null.
    /// </returns>
    public async Task<long?> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        var result = await Collection.DeleteManyAsync(predicate);

        if (result.IsAcknowledged)
        {
            return result.DeletedCount;
        }

        return null;
    }

    /// <summary>
    /// Deletes all entities from the MongoDB collection.
    /// </summary>
    /// <returns>A task representing the asynchronous delete operation.</returns>
    public Task DeleteAllAsync()
    {
        return Collection.DeleteManyAsync(x => true);
    }

    /// <summary>
    /// Soft deletes a single entity in the MongoDB collection by its ID. The entity is not actually removed from the collection but is marked as deleted.
    /// </summary>
    /// <param name="id">The ID of the entity to soft delete.</param>
    /// <returns>A task representing the asynchronous soft delete operation.</returns>
    public Task SoftDeleteAsync(string id)
    {
        var update = UpdateBuilder.Set(x => x.IsSoftDeleted, true);
        var filter = IdFilter(id);
        return Collection.UpdateOneAsync(filter, update);
    }

    /// <summary>
    /// Counts the number of entities in the MongoDB collection with a specified ID.
    /// </summary>
    /// <param name="id">The ID to count entities for.</param>
    /// <returns>The number of entities with the specified ID.</returns>
    public Task<long> CountAsync(string id)
    {
        var filter = IdFilter(id);
        return Collection.CountDocumentsAsync(filter);
    }

    /// <summary>
    /// Counts the number of entities in the MongoDB collection that match a specified selector.
    /// </summary>
    /// <param name="selector">The selector to determine which entities to count.</param>
    /// <returns>The number of entities that match the selector.</returns>
    public Task<long> CountAsync(Expression<Func<T, bool>> selector)
    {
        var filter = FilterBuilder.Where(selector);
        return Collection.CountDocumentsAsync(filter);
    }

    /// <summary>
    /// Counts all entities in the MongoDB collection.
    /// </summary>
    /// <returns>The total number of entities in the collection.</returns>
    public Task<long> CountAllAsync()
    {
        return Collection.CountDocumentsAsync(EmptyFilter);
    }

    /// <summary>
    /// Counts the number of entities in the MongoDB collection that match a specified filter.
    /// </summary>
    /// <param name="filter">The filter to determine which entities to count.</param>
    /// <returns>The number of entities that match the filter.</returns>
    public virtual Task<long> MongoCountAsync(FilterDefinition<T> filter)
    {
        return Collection.CountDocumentsAsync(filter);
    }

    /// <summary>
    /// Validates the format of a string ID to ensure it is a valid MongoDB ObjectId.
    /// </summary>
    /// <param name="id">The string ID to validate.</param>
    /// <returns><c>true</c> if the ID is a valid ObjectId format; otherwise, <c>false</c>.</returns>
    public bool ValidateIdFormat(string id)
    {
        return ObjectId.TryParse(id, out var _);
    }

    /// <summary>
    /// Validates a string ID to ensure it corresponds to an existing entity in the MongoDB collection.
    /// </summary>
    /// <param name="id">The string ID to validate.</param>
    /// <returns><c>true</c> if the ID corresponds to an existing entity; otherwise, <c>false</c>.</returns>
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
