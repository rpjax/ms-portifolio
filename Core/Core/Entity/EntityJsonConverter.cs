using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Core;

public class EntityJsonConverter<T> : JsonConverter<T>
{
    private ISerializer<T> Serializer { get; }

    public EntityJsonConverter(ISerializer<T> serializer)
    {
        Serializer = serializer;
    }

    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            var json = root.ToString();
            return Serializer.Deserialize(json);
        }
    }

    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        writer.WriteRawValue(Serializer.Serialize(value));
    }
}