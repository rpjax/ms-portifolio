using System.Text.Json.Serialization;

namespace ModularSystem.Core;

public static partial class EntityConfigurationExtensions
{
    public static JsonConverter<T>? TryGetJsonConverter<T>(this EntityConfiguration<T> configuration) where T : class, IQueryableModel
    {
        var serializer = configuration.GetSerializer();

        if (serializer == null)
        {
            return null;
        }

        return new EntityJsonConverter<T>(serializer);
    }

}
