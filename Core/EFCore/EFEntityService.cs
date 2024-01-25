using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Provides a base class for services managing entities within an Entity Framework context. <br/>
/// This class offers common functionalities for entity operations.
/// </summary>
/// <typeparam name="T">The type of the entity, which should implement the <see cref="IEFModel"/> interface.</typeparam>
public abstract class EFEntityService<T> : EntityService<T> where T : class, IEFModel
{
    /// <summary>
    /// Creates an expression to select the ID of an entity based on a given parameter. <br/>
    /// </summary>
    /// <param name="parameter">The parameter expression used as the source for accessing the entity's properties.</param>
    /// <returns>A <see cref="MemberExpression"/> representing the ID property of the entity, typically used in LINQ queries.</returns>
    /// <remarks>
    /// This method constructs a member access expression for the "Id" property, assuming that the entity type <typeparamref name="T"/> implements <see cref="IEFModel"/> which includes an "Id" property.
    /// </remarks>
    protected override MemberExpression CreateIdSelectorExpression(ParameterExpression parameter)
    {
        return Expression.Property(parameter, nameof(IEFModel.Id));
    }

    /// <summary>
    /// Attempts to convert a string representation of an entity's ID into its corresponding type. <br/>
    /// </summary>
    /// <param name="id">The string representation of the entity's ID.</param>
    /// <returns>The converted ID as an object if successful, otherwise null.</returns>
    /// <remarks>
    /// This implementation is designed for entity IDs of type <see cref="long"/>. <br/>
    /// It tries to parse the string ID into a long. If successful, the parsed long value is returned; otherwise, it returns null. <br/>
    /// </remarks>
    protected override object? TryParseId(string id)
    {
        if (long.TryParse(id, out long value))
        {
            return value;
        }

        return null;
    }
}
