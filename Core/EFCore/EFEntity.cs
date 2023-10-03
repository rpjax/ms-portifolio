using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.EntityFramework;

/// <summary>
/// Provides a base for entities in an Entity Framework context.
/// </summary>
/// <typeparam name="T">The type of the entity, which should implement the <see cref="IEFModel"/> interface.</typeparam>
public abstract class EFEntity<T> : EntityService<T> where T : class, IEFModel
{
    /// <summary>
    /// Creates an expression to select the ID of an entity.
    /// </summary>
    /// <param name="parameter">The parameter expression used as the source for the member access.</param>
    /// <returns>A <see cref="MemberExpression"/> representing the ID property of the entity.</returns>
    /// <remarks>
    /// This method relies on the fact that the entity type <typeparamref name="T"/> should have an "Id" property, as declared in <see cref="IEFModel"/>.
    /// </remarks>
    protected override MemberExpression CreateIdSelectorExpression(ParameterExpression parameter)
    {
        return Expression.Property(parameter, nameof(IEFModel.Id));
    }

    /// <summary>
    /// Tries to parse the provided string ID into an appropriate type.
    /// </summary>
    /// <param name="id">The string representation of the entity's ID.</param>
    /// <returns>The parsed ID if the parsing was successful, otherwise null.</returns>
    /// <remarks>
    /// This implementation assumes that IDs in the Entity Framework model are of type <see cref="long"/>.
    /// If the provided string ID can be parsed into a long, the parsed value is returned; otherwise, null is returned.
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