using System.Collections.Concurrent;

namespace ModularSystem.Core;

/// <summary>
/// Represents the base configuration for entities, providing caching functionality to manage 
/// and retrieve entity configurations.
/// </summary>
/// <remarks>
/// Derived classes should provide specific configuration details for individual entities.
/// </remarks>
public abstract class EntityConfiguration
{
    /// <summary>
    /// Gets the cache storing entity configurations, indexed by the full name of their type.
    /// </summary>
    public static ConcurrentDictionary<string, EntityConfiguration> ConfigurationCache { get; } = new();

    /// <summary>
    /// Adds a given entity configuration to the cache.
    /// <param name="key">The key index of the entity configuration to be added.</param>
    /// </summary>
    /// <param name="configuration">The entity configuration to be added.</param>
    /// <exception cref="InvalidOperationException">
    /// Thrown if the type of the provided entity configuration does not have a full name. 
    /// This situation is not expected to occur under normal conditions.
    /// </exception>
    public static void AddConfiguration(string key, EntityConfiguration configuration)
    {
        ConfigurationCache.TryAdd(key, configuration);
    }

    /// <summary>
    /// Attempts to retrieve the entity configuration for a given type from the cache.
    /// </summary>
    /// <param name="type">The type of the entity configuration to retrieve.</param>
    /// <returns>
    /// The <see cref="EntityConfiguration"/> for the provided type if found; otherwise, <c>null</c>.
    /// </returns>
    public static EntityConfiguration? TryGetConfiguration(Type type)
    {
        if (type.FullName == null)
        {
            return null;
        }

        return ConfigurationCache.TryGetValue(type.FullName, out var serializer) ? serializer : null;
    }

    /// <summary>
    /// Attempts to retrieve the entity configuration for a generic entity type from the cache.
    /// </summary>
    /// <typeparam name="T">The type of the entity for which the configuration is to be retrieved.</typeparam>
    /// <returns>
    /// The <see cref="EntityConfiguration{T}"/> for the provided entity type if found; otherwise, <c>null</c>.
    /// </returns>
    public static EntityConfiguration? TryGetConfiguration<T>()
    {
        return TryGetConfiguration(typeof(T))?.TryTypeCast<EntityConfiguration>();
    }

    public static IEnumerable<EntityConfiguration> All()
    {
        return ConfigurationCache.Values;
    }
}


public abstract class EntityConfiguration<T> : EntityConfiguration where T : class, IQueryableModel
{
    public virtual ISerializer<T>? GetSerializer()
    {
        return new DefaultEntityJsonSerializer<T>();
    }
}