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
    public async Task<IMongoQueryable<T>> CreateMongoQueryableAsync()
    {
        return (IMongoQueryable<T>)await CreateQueryAsync();
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
