using System.Reflection;

namespace ModularSystem.Core;

/// <summary>
/// Provides utility methods for reflection-based operations on entities and their configurations.
/// </summary>
public static class EntityReflection
{
    /// <summary>
    /// Retrieves all non-abstract entity types that derive from the generic <see cref="Entity{T}"/> from the specified assemblies.
    /// </summary>
    /// <param name="assemblies">A collection of assemblies to search for entity types.</param>
    /// <returns>An enumeration of found entity types.</returns>
    public static IEnumerable<Type> GetAllEntitesFrom(IEnumerable<Assembly> assemblies)
    {
        var abstractEntityType = typeof(Entity<>);

        return assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract && type.IsSubclassOfRawGeneric(abstractEntityType))
            .ToArray();
    }

    /// <summary>
    /// Retrieves all non-abstract entity configuration types that derive from the generic <see cref="EntityConfiguration{T}"/> from the specified assemblies.
    /// </summary>
    /// <param name="assemblies">A collection of assemblies to search for entity configuration types.</param>
    /// <returns>An enumeration of found entity configuration types.</returns>
    public static IEnumerable<Type> GetAllEntityConfigurationsFrom(IEnumerable<Assembly> assemblies)
    {
        var abstractEntityConfigType = typeof(EntityConfiguration<>);

        return assemblies
            .SelectMany(assembly => assembly.GetTypes())
            .Where(type => !type.IsAbstract && type.IsSubclassOfRawGeneric(abstractEntityConfigType))
            .ToList();
    }

    /// <summary>
    /// Extracts and returns the model type argument from the provided entity type,<br/>
    /// assuming the entity type inherits from <see cref="Entity{T}"/>.
    /// </summary>
    /// <param name="entityType">The entity type to extract the model type argument from.</param>
    /// <returns>The model type argument.</returns>
    public static Type GetModelTypeFromEntityType(Type entityType)
    {
        return entityType.BaseType.GenericTypeArguments[0];
    }
}