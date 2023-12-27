namespace ModularSystem.Core.Linq;

/// <summary>
/// Defines an extended IQueryable interface that includes additional query capabilities.
/// </summary>
public interface IExtendedQueryable : IQueryable
{
}

/// <summary>
/// Defines an extended IQueryable interface for a specific type, including additional query capabilities and asynchronous operations.
/// </summary>
/// <typeparam name="T">The type of the elements in the queryable sequence.</typeparam>
public interface IExtendedQueryable<T> : IExtendedQueryable, IQueryable<T>
{
    /// <summary>
    /// Asynchronously creates an array from the IQueryable sequence.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation and contains the array of queried elements.</returns>
    Task<T[]> ToArrayAsync();
}
