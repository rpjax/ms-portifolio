using ModularSystem.Core.Logging;

namespace ModularSystem.Core;

public class EntityInitializer : Initializer
{
    public EntityInitializer()
    {
        //*
        // Low priority because this initializer is super expensive.
        //*
        Priority = (int)Core.PriorityLevel.High;
    }

    /// <summary>
    /// Registers entity-specific JSON serializers for concrete implementations of the generic <see cref="EntityService{T}"/> type.
    /// </summary>
    /// <param name="options">Configuration options.</param>
    public override async Task InternalInitAsync(Options options)
    {
        if (!options.InitializeEntityConfigurations)
        {
            return;
        }

        //*
        // this feture is disabled.
        //*

        ConsoleLogger.Warn("EntityConfiguration initialization has been disabled duo to development issues.");
        return;

        var abstractEntityType = typeof(EntityService<>);
        var abstractEntityConfigType = typeof(EntityConfiguration<>);

        var entityTypes = EntityReflection.GetAllEntitesFrom(options.Assemblies).ToArray();

        var entityConfigTypes = EntityReflection.GetAllEntityConfigurationsFrom(options.Assemblies)
            .ToArray();

        foreach (var entityType in entityTypes)
        {
            //*
            // looks for the implementation of configuration with closest type in object inheritance tree,
            // then maps it to the concrete type.
            //*

            if (entityType.BaseType.GenericTypeArguments.IsEmpty())
            {
                continue;
            }

            if (entityType.FullName == null)
            {
                throw new Exception("The entity does not have a class name.");
            }

            var modelType = entityType.BaseType.GenericTypeArguments[0];

            var configModels = entityConfigTypes
                .Where(type => type.BaseType.GenericTypeArguments.IsNotEmpty())
                .Where(type => modelType.IsSubclassOf(type.BaseType.GenericTypeArguments.First()) || modelType == type.BaseType.GenericTypeArguments.First())
                .OrderByDescending(type => type.BaseType.GenericTypeArguments.First().InheritanceDistance(modelType))
                .ToArray();

            if (configModels.IsEmpty())
            {
                var _type = typeof(DefaultEntityConfiguration<>).MakeGenericType(modelType);
                var _default = Activator.CreateInstance(_type)?.TryTypeCast<EntityConfiguration>();

                if (_default == null)
                {
                    throw new Exception($"Could not create default entity configuration for entity of type: \"{entityType.FullName}\".");
                }

                EntityConfiguration.AddConfiguration(entityType.FullName, _default);
                continue;
            }

            var closestConfigType = configModels.First();
            var closestConfigInstance = Activator.CreateInstance(closestConfigType)?.TypeCast<EntityConfiguration>();

            if (closestConfigInstance == null)
            {
                throw new Exception($"Could not create instance of entity configuration of type: \"{closestConfigType.FullName}\".");
            }

            EntityConfiguration.AddConfiguration(entityType.FullName, closestConfigInstance);
        }

        ConsoleLogger.Info("Successfully created all Entity Configurations.");
    }
}
