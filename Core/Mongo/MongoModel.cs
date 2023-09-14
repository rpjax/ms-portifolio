﻿using ModularSystem.Core;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ModularSystem.Mongo;

/// <summary>
/// Represents a model designed for MongoDB storage, with querying capabilities and identified by an <see cref="ObjectId"/>.
/// </summary>
/// <remarks>
/// Extends <see cref="IQueryableModel"/> to provide MongoDB-specific identifier properties. 
/// </remarks>
public interface IMongoModel : IQueryableModel
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
/// Represents a base MongoDB model that supports querying capabilities defined in <see cref="IQueryableModel"/>.
/// </summary>
/// <remarks>
/// This abstract class extends <see cref="QueryableModel"/> to offer MongoDB-specific features, notably the usage of ObjectId as a unique identifier.
/// </remarks>
public abstract class MongoModel : QueryableModel, IMongoModel
{
    /// <summary>
    /// Represents the MongoDB ObjectId that serves as a unique identifier for the model.
    /// </summary>
    /// <remarks>
    /// The ObjectId is a 12-byte identifier typically employed by MongoDB to uniquely identify documents within a collection.
    /// </remarks>
    [BsonId]
    public ObjectId Id { get; set; }

    /// <summary>
    /// Converts the MongoDB ObjectId to its string representation and retrieves it.
    /// </summary>
    /// <returns>The string representation of the MongoDB ObjectId.</returns>
    public override string GetId()
    {
        return Id.ToString();
    }

    /// <summary>
    /// Parses and sets the MongoDB ObjectId using its string representation.
    /// </summary>
    /// <param name="id">The string representation of the MongoDB ObjectId to be assigned to the model.</param>
    /// <remarks>
    /// An exception will be thrown if the provided string is not a valid representation of an ObjectId.
    /// </remarks>
    public override void SetId(string id)
    {
        Id = ObjectId.Parse(id);
    }
}
