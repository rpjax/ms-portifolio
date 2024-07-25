namespace ModularSystem.EntityFramework;

/// <summary>
/// Interface for Entity Framework models, providing a unique identifier of type <see cref="long"/>.
/// </summary>
public interface IEFCoreModel
{
    /// <summary>
    /// Unique identifier for the entity, used by Entity Framework.
    /// </summary>
    long Id { get; }
}

