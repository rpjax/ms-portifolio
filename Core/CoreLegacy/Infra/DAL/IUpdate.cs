using System.Linq.Expressions;

namespace ModularSystem.Core;


/// <summary>
/// Represents an interface for updating entities.
/// </summary>
/// <typeparam name="T">The type of entity.</typeparam>
public interface IUpdate<T>
{
    /// <summary>
    /// Gets the filter expression used to filter the entities to be updated.
    /// </summary>
    Expression? Filter { get; }

    /// <summary>
    /// Gets an array of expressions representing the modifications to be applied to the entities.
    /// </summary>
    Expression[] Modifications { get; }
}
