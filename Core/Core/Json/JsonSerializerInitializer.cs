using ModularSystem.Core.Logging;
using System.Text.Json.Serialization;

namespace ModularSystem.Core;

internal class JsonSerializerInitializer : Initializer
{
    public JsonSerializerInitializer()
    {
        Priority = (int)Core.Priority.Normal;
    }

    public override void OnInit(Options options)
    {
        if (options.JsonSerialization.UseEntityConverters)
        {
            //*
            // this feture is disabled.
            //*

            ConsoleLogger.Warn("Entity converters initialization has been disabled duo to development issues.");
            //RegisterEntityJsonConverters(options);
        }

        if (options.JsonSerialization.UseUtcDateTimeConverter)
        {
            RegisterUtcDateTimeJsonConverter();
        }
    }

    void RegisterEntityJsonConverters(Options options)
    {
        foreach (var entityType in EntityReflection.GetAllEntitesFrom(options.Assemblies))
        {
            var entityConfigurationInstance = EntityConfiguration.TryGetConfiguration(entityType);

            if (entityConfigurationInstance == null)
            {
                continue;
            }

            var entityModelType = EntityReflection.GetModelTypeFromEntityType(entityType);

            var method = entityConfigurationInstance.GetType().GetMethod(nameof(EntityConfiguration<QueryableModel>.GetSerializer));
            var serializer = method.Invoke(entityConfigurationInstance, null);

            if (serializer == null)
            {
                throw new Exception($"Cold not create serializer from entity configuration of type: \"{entityConfigurationInstance.GetType().FullName}\".");
            }

            var configType = entityConfigurationInstance.GetType();

            if (configType.BaseType.GenericTypeArguments.IsEmpty())
            {
                throw new Exception($"Cold not create serializer from entity configuration of type: \"{entityConfigurationInstance.GetType().FullName}\".");
            }

            var configModelType = configType.BaseType.GenericTypeArguments[0];

            var getterMethodInfo = typeof(EntityConfigurationExtensions)
                .GetMethod(nameof(EntityConfigurationExtensions.TryGetJsonConverter))
                ?.MakeGenericMethod(configModelType);

            //EntityConfigurationExtensions.TryGetJsonConverter<MongoAnt>()

            var jsonConverter = getterMethodInfo
                .Invoke(null, new object[] { entityConfigurationInstance })
                ?.TryTypeCast<JsonConverter>();

            if (jsonConverter == null)
            {
                throw new Exception($"Cold not create serializer from entity configuration of type: \"{entityConfigurationInstance.GetType().FullName}\".");
            }

            if (configModelType.FullName == null)
            {
                throw new Exception($"Cold not create serializer from entity configuration of type: \"{entityConfigurationInstance.GetType().FullName}\".");
            }

            JsonSerializerSingleton.TryAddConverter(configModelType.FullName, jsonConverter);
        }

        ConsoleLogger.Info("Successfully added all Entity JsonConverters to ASP.NET's JsonSerializer.");
    }

    void RegisterUtcDateTimeJsonConverter()
    {
        JsonSerializerSingleton.TryAddConverter(typeof(DateTime), new UtcDateTimeConverter());
    }
}
