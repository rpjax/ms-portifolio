using ModularSystem.Core;
using ModularSystem.Core.Linq;
using ModularSystem.Mongo.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using ModularSystem.Core.Linq.Extensions;
using ModularSystem.Core.Linq.Expressions;
using System.Linq.Expressions;

namespace ModularSystem.Mongo.Repositories;

/// <summary>
/// Provides a repository for managing MongoDB collections.
/// </summary>
/// <typeparam name="T">The type of the entity.</typeparam>
public class MongoRepository<T> : IMongoRepository<T> where T : IMongoModel
{
    //*
    // Helpers
    //*

    /// <summary>
    /// Gets a filter definition builder for the entity type.
    /// </summary>
    protected FilterDefinitionBuilder<T> FilterBuilder => new FilterDefinitionBuilder<T>();

    /// <summary>
    /// Gets an update definition builder for the entity type.
    /// </summary>
    protected UpdateDefinitionBuilder<T> UpdateBuilder => new UpdateDefinitionBuilder<T>();

    private IMongoCollection<T> Collection { get; }
    private IClientSessionHandle? Session { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MongoRepository{T}"/> class.
    /// </summary>
    /// <param name="collection"></param>
    public MongoRepository(IMongoCollection<T> collection)
    {
        Collection = collection;
        Session = null;
    }

    /// <summary>
    /// Disposes of the repository.
    /// </summary>
    /// <exception cref="NotImplementedException"></exception>
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Asynchronously inserts a single entity into the collection.
    /// </summary>
    /// <param name="entity">The entity to insert.</param>
    public async Task CreateAsync(T entity)
    {
        if (Session != null)
        {
            await Collection.InsertOneAsync(Session, entity);
        }
        else
        {
            await Collection.InsertOneAsync(entity);
        }
    }

    /// <summary>
    /// Asynchronously inserts multiple entities into the collection.
    /// </summary>
    /// <param name="entities">The entities to insert.</param>
    public async Task CreateAsync(IEnumerable<T> entities)
    {
        if (Session != null)
        {
            await Collection.InsertManyAsync(Session, entities);
        }
        else
        {
            await Collection.InsertManyAsync(entities);
        }
    }

    //*
    // Read
    //*

    /// <summary>
    /// Provides an asynchronous queryable interface to the collection.
    /// </summary>
    /// <returns>An IAsyncQueryable of the entity type.</returns>
    public IAsyncQueryable<T> AsQueryable()
    {
        return new MongoAsyncQueryable<T>(Collection.AsQueryable());
    }

    /// <summary>
    /// Tries to asynchronously find an entity by its ID.
    /// </summary>
    /// <param name="id">The ID of the entity.</param>
    /// <returns>The entity if found; otherwise, null.</returns>
    public Task<T?> GetAsync(ObjectId id)
    {
        return AsQueryable()
            .Where(x => x.Id == id)
            .FirstOrDefaultAsync();
    }

    //*
    // Update
    //*

    /// <summary>
    /// Asynchronously replaces an entity in the collection.
    /// </summary>
    /// <param name="entity">The entity to replace.</param>
    public async Task UpdateAsync(T entity)
    {
        var filter = CreateIdFilter(entity.Id);

        if (Session != null)
        {
            await Collection.ReplaceOneAsync(Session, filter, entity);
        }
        else
        {
            await Collection.ReplaceOneAsync(filter, entity);
        }
    }

    /// <summary>
    /// Asynchronously updates entities in the collection based on a provided update definition.
    /// </summary>
    /// <param name="update">The update definition.</param>
    /// <returns>The number of modified entities.</returns>
    /// <exception cref="ErrorException">Thrown when the update operation is not properly defined.</exception>
    public async Task<long> UpdateAsync(IUpdateExpression update)
    {
        throw new NotImplementedException();
        //var reader = new UpdateExpressionReader<T>(update);

        //var predicate = reader
        //    .GetFilterExpression();

        //var modifications = reader
        //    .GetModificationExpressions()
        //    .ToArray();

        //if (predicate == null)
        //{
        //    throw new Exception("Update operation requires a filter definition. To allow updates without a filter, adjust the settings in the provided configuration object when initializing the data access object.");
        //}
        //if (modifications == null || modifications.IsEmpty())
        //{
        //    return 0;
        //}

        //var filterDefinition = FilterBuilder.Where(predicate);
        //var updateDefinition = UpdateBuilder.Combine();

        //foreach (var updateSet in modifications)
        //{
        //    var valueExpression = updateSet.Value;
        //    var value = valueExpression as object;

        //    if (value is ConstantExpression constantExpression)
        //    {
        //        value = constantExpression.Value;
        //    }

        //    var set = UpdateBuilder
        //        .Set(updateSet.FieldName, value);

        //    updateDefinition = UpdateBuilder
        //        .Combine(updateDefinition, set);
        //}

        //var result = await Collection.UpdateManyAsync(filterDefinition, updateDefinition);

        //if (!result.IsAcknowledged || !result.IsModifiedCountAvailable)
        //{
        //    throw new Exception("The operation was not acknowledged by the mongo database.");
        //}

        //return result.ModifiedCount;
    }

    //*
    // Delete
    //*

    /// <summary>
    /// Asynchronously deletes a single entity from the collection.
    /// </summary>
    /// <param name="entity">The entity to delete.</param>
    public async Task DeleteAsync(T entity)
    {
        var filter = CreateIdFilter(entity.Id);

        if (Session != null)
        {
            await Collection.DeleteOneAsync(Session, filter);
        }
        else
        {
            await Collection.DeleteOneAsync(filter);
        }
    }

    /// <summary>
    /// Asynchronously deletes entities from the collection based on a predicate.
    /// </summary>
    /// <param name="predicate">The predicate to determine which entities to delete.</param>
    /// <returns>The number of deleted entities.</returns>
    public async Task<long> DeleteAsync(Expression<Func<T, bool>> predicate)
    {
        var filter = CreateFilterFromPredicate(predicate);
        var result = null as DeleteResult;

        if (Session != null)
        {
            result = await Collection.DeleteManyAsync(Session, filter);
        }
        else
        {
            result = await Collection.DeleteManyAsync(filter);
        }

        return result.DeletedCount;
    }

    //*
    // Transaction functions
    //*

    /// <summary>
    /// Binds a client session to the repository for transactions.
    /// </summary>
    /// <param name="session">The session to bind.</param>
    public void BindSession(IClientSessionHandle session)
    {
        Session = session;
    }

    /// <summary>
    /// Unbinds the current session from the repository.
    /// </summary>
    public void UnbindSession()
    {
        Session = null;
    }

    //*
    // Internal Helpers
    //*


    /// <summary>
    /// Creates a filter definition for an entity by its ID.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    protected FilterDefinition<T> CreateIdFilter(ObjectId id)
    {
        return new FilterDefinitionBuilder<T>()
            .Eq(x => x.Id, id);
    }

    /// <summary>
    /// Creates a filter definition from a predicate.
    /// </summary>
    /// <param name="predicate"></param>
    /// <returns></returns>
    protected FilterDefinition<T> CreateFilterFromPredicate(Expression<Func<T, bool>> predicate)
    {
        return new FilterDefinitionBuilder<T>()
            .Where(predicate);
    }

}
