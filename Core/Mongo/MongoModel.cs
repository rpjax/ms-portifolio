using ModularSystem.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModularSystem.Mongo;

/// <summary>
/// Defines an interface for MongoDB entities identified by an ObjectId.
/// </summary>
public interface IMongoModel 
{
    /// <summary>
    /// Gets the unique MongoDB ObjectId associated with the model.
    /// </summary>
    /// <remarks>
    /// ObjectId is a 12-byte identifier typically employed by MongoDB to uniquely identify documents within a collection.
    /// </remarks>
    ObjectId Id { get; }
}

/// <summary>
/// Defines an interface for MongoDB entities identified by a GUID.
/// </summary>
/// <remarks>
/// This interface extends <see cref="IEntity"/> to accommodate models that utilize a GUID as their primary identifier, <br/>
/// allowing for unique identification within a MongoDB collection in scenarios where an <see cref="ObjectId"/> is not used.
/// </remarks>
public interface IMongoGuidModel 
{
    /// <summary>
    /// Gets the GUID identifier for the model.
    /// </summary>
    Guid Id { get; }
}

/// <summary>
/// Represents a base MongoDB model that supports querying capabilities defined in <see cref="IEntity"/>.
/// </summary>
/// <remarks>
/// This abstract class extends <see cref="Entity"/> to offer MongoDB-specific features, <br/>
/// notably the usage of ObjectId as a unique identifier.
/// </remarks>
public abstract class MongoModel : IMongoModel
{
    /// <summary>
    /// Represents the MongoDB ObjectId that serves as a unique identifier for the model.
    /// </summary>
    /// <remarks>
    /// The ObjectId is a 12-byte identifier typically employed by MongoDB to uniquely identify documents within a collection.
    /// </remarks>
    [BsonId]
    public ObjectId Id { get; set; }
}

/// <summary>
/// Represents a MongoDB entity with a GUID as its identifier.
/// </summary>
/// <remarks>
/// This abstract class is designed for models that require a GUID for identification instead of the traditional ObjectId used by MongoDB. <br/>
/// Utilizing GUIDs can be beneficial for systems that need to generate unique identifiers independently of the database.
/// </remarks>
public abstract class MongoGuidModel : IMongoGuidModel
{
    /// <summary>
    /// Gets or sets the GUID used as the unique identifier for the model.
    /// </summary>
    /// <value>
    /// The GUID identifier.
    /// </value>
    [BsonId]
    public Guid Id { get; set; }

}