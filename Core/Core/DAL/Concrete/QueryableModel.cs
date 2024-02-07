namespace ModularSystem.Core;

/// <summary>
/// Provides a base implementation of the <see cref="IQueryableModel"/> interface 
/// for models that support querying capabilities.
/// </summary>
/// <remarks>
/// This abstract class can be derived to create models that can be queried, 
/// compared, and have built-in soft deletion support. <br/>
/// The created time is set
/// automatically upon instantiation.
/// </remarks>
[Serializable]
public abstract class QueryableModel : IQueryableModel
{
    /// <summary>
    /// Gets or sets a value indicating whether the model is soft deleted.
    /// </summary>
    /// <remarks>
    /// Soft deletion is a strategy in which records are marked as deleted, rather than removing them from the database.
    /// </remarks>
    public bool IsSoftDeleted { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the model was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this model was last modified.
    /// </summary>
    public DateTime LastModifiedAt { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="QueryableModel"/> class.
    /// </summary>
    public QueryableModel()
    {
        CreatedAt = TimeProvider.UtcNow();
        LastModifiedAt = TimeProvider.UtcNow();
    }

    /// <summary>
    /// Retrieves the identifier of the model.
    /// </summary>
    /// <returns>The identifier of the model as a string.</returns>
    public abstract string GetId();

    /// <summary>
    /// Determines whether the current object is equal to another object of the same type.
    /// </summary>
    /// <param name="other">An object to compare with this object.</param>
    /// <returns>
    /// <c>true</c> if the current object is equal to the <paramref name="other"/> parameter; otherwise, <c>false</c>.
    /// </returns>
    public bool Equals(IQueryableModel? other)
    {
        if (other == null)
        {
            return false;
        }

        return GetId() == other.GetId();
    }
}
