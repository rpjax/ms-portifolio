namespace ModularSystem.Core;

/// <summary>
/// Represents a model that supports querying capabilities.
/// </summary>
/// <remarks>
/// This interface extends the IEquatable of the same type to ensure that <br/>
/// instances of implementing classes can be compared for equality based on their contents.
/// </remarks>
public interface IEntity : IEquatable<IEntity>
{
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

}
