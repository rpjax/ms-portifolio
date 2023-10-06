using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Represents update instructions for entities of type <typeparamref name="T"/>.
/// Contains filtering criteria and modifications to be applied.
/// </summary>
/// <typeparam name="T">Type of entity to be updated.</typeparam>
public interface IUpdate<T>
{
    /// <summary>
    /// Specifies which entities should be targeted for the update operation.
    /// </summary>
    Expression? Filter { get; set; }

    /// <summary>
    /// Contains expressions detailing the modifications to be made on the target entities.
    /// </summary>
    List<Expression> Modifications { get; set; }
}
