using ModularSystem.Core;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using System.Linq.Expressions;

namespace ModularSystem.Mongo;

/// <summary>
/// Provides a base for entities in a MongoDB context.
/// </summary>
/// <typeparam name="T">The type of the entity, which should implement the <see cref="IMongoModel"/> interface.</typeparam>
public abstract class MongoEntityService<T> : EntityService<T> where T : class, IMongoModel
{
    /// <summary>
    /// Asynchronously creates an <see cref="IMongoQueryable{T}"/> instance for querying the MongoDB collection of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation. The task result contains an <see cref="IMongoQueryable{T}"/> instance, <br/>
    /// which allows for building LINQ queries against the MongoDB collection of entities of type <typeparamref name="T"/>.
    /// </returns>
    /// <remarks>
    /// This method internally calls <c>CreateQueryableAsync</c>, which should be implemented to asynchronously create the base query
    /// for the MongoDB collection. <br/>
    /// The returned <see cref="IMongoQueryable{T}"/> is cast from the result of <c>CreateQueryableAsync</c>.
    /// </remarks>
    public IMongoQueryable<T> CreateMongoQueryable()
    {
        return (IMongoQueryable<T>)DataAccessObject.AsQueryable();
    }

    /// <summary>
    /// Creates an expression to select the ID of an entity in the MongoDB context.
    /// </summary>
    /// <param name="parameter">The parameter expression used as the source for the member access.</param>
    /// <returns>A <see cref="MemberExpression"/> representing the ID property of the entity.</returns>
    /// <remarks>
    /// This method leverages the fact that the entity type <typeparamref name="T"/> has an "Id" property, as declared in <see cref="IMongoModel"/>.
    /// </remarks>
    protected override MemberExpression CreateIdSelectorExpression(ParameterExpression parameter)
    {
        return Expression.Property(parameter, nameof(IMongoModel.Id));
    }

    /// <summary>
    /// Tries to parse the provided string ID into an appropriate ObjectId used in MongoDB.
    /// </summary>
    /// <param name="id">The string representation of the entity's ID.</param>
    /// <returns>The parsed ObjectId if the parsing was successful, otherwise null.</returns>
    /// <remarks>
    /// MongoDB typically uses <see cref="ObjectId"/> for IDs. This method attempts to parse a string representation of an ID into an ObjectId.
    /// If the parsing is successful, the ObjectId is returned; otherwise, null is returned.
    /// </remarks>
    protected override object? TryParseId(string id)
    {
        if (ObjectId.TryParse(id, out var value))
        {
            return value;
        }

        return null;
    }

}

/// <summary>
/// Provides services for entities with a GUID identifier in a MongoDB collection.
/// </summary>
/// <typeparam name="T">The entity type with a GUID identifier.</typeparam>
public abstract class MongoGuidEntityService<T> : EntityService<T> where T : class, IMongoGuidModel
{
    /// <summary>
    /// Creates a queryable object for MongoDB collection entities of type <typeparamref name="T"/>.
    /// </summary>
    /// <returns>An <see cref="IMongoQueryable{T}"/> for LINQ queries.</returns>
    /// <remarks>
    /// Utilizes <c>CreateQueryableAsync</c> for asynchronous query creation.
    /// </remarks>
    public IMongoQueryable<T> CreateMongoQueryable()
    {
        return (IMongoQueryable<T>)DataAccessObject.AsQueryable();
    }

    /// <summary>
    /// Creates an expression for selecting the ID of an entity.
    /// </summary>
    /// <param name="parameter">The parameter expression.</param>
    /// <returns>An expression selecting the ID.</returns>
    protected override MemberExpression CreateIdSelectorExpression(ParameterExpression parameter)
    {
        return Expression.Property(parameter, nameof(IMongoGuidModel.Id));
    }

    /// <summary>
    /// Attempts to parse a string ID to a GUID.
    /// </summary>
    /// <param name="id">The string ID.</param>
    /// <returns>The parsed GUID or null.</returns>
    protected override object? TryParseId(string id)
    {
        if (Guid.TryParse(id, out var value))
        {
            return value;
        }

        return null;
    }
}
