namespace ModularSystem.Core;

/// <summary>
/// Represents a model that supports querying capabilities.
/// </summary>
/// <remarks>
/// This interface extends the IEquatable of the same type to ensure that <br/>
/// instances of implementing classes can be compared for equality based on their contents.
/// </remarks>
public interface IQueryableModel : IEquatable<IQueryableModel>
{
    /// <summary>
    /// Gets or sets a value indicating whether the model is soft deleted.
    /// </summary>
    /// <remarks>
    /// Soft deletion is a strategy in which records are marked as deleted, rather than removing them from the database.
    /// </remarks>
    bool IsSoftDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the model was created.
    /// </summary>
    DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this model was last modified.
    /// </summary>
    DateTime LastModifiedAt { get; set; }

    /// <summary>
    /// Retrieves the identifier of the model.
    /// </summary>
    /// <returns>The identifier of the model as a string.</returns>
    string GetId();

    /// <summary>
    /// Sets the identifier of the model.
    /// </summary>
    /// <param name="id">The identifier to be set.</param>
    void SetId(string id);
}
