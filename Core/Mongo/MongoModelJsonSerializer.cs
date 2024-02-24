using ModularSystem.Core;
using MongoDB.Bson;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Mongo;

/// <summary>
/// Provides serialization and deserialization methods for MongoDB models.
/// </summary>
/// <typeparam name="T">The type of the MongoDB model to be serialized/deserialized.</typeparam>
public class MongoModelJsonSerializer<T> : ISerializer<T> where T : class, IMongoEntity
{
    /// <summary>
    /// Serializes the given object into a JSON string.
    /// </summary>
    /// <typeparam name="TObject">The type of the object to be serialized.</typeparam>
    /// <param name="input">The object to serialize.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public string Serialize(T input)
    {
        var options = new JsonSerializerOptions()
            .AddConverter(new ObjectIdJsonConverter());

        return JsonSerializerSingleton.Serialize(input, options);
    }

    /// <summary>
    /// Tries to deserialize a JSON string into an object of type T. Returns null if the deserialization fails.
    /// </summary>
    /// <param name="input">The JSON string to deserialize.</param>
    /// <returns>An instance of type T or null.</returns>
    public T? TryDeserialize(string input)
    {
        var options = new JsonSerializerOptions()
            .AddConverter(new ObjectIdJsonConverter());

        var obj = JsonSerializerSingleton.Deserialize<T>(input, options);

        return obj;
    }

    /// <summary>
    /// Deserializes a JSON string into an object of type T. Throws an exception if the deserialization fails.
    /// </summary>
    /// <param name="serialized">The JSON string to deserialize.</param>
    /// <returns>An instance of type T.</returns>
    public T Deserialize(string serialized)
    {
        var obj = TryDeserialize(serialized);

        if (obj == null)
        {
            throw new AppException($"Could not deserialize the given JSON to an instance of type: {typeof(T).FullName}.");
        }

        return obj;
    }
}

/// <summary>
/// A custom JSON converter for MongoDB's <see cref="ObjectId"/> type.
/// </summary>
public class ObjectIdJsonConverter : JsonConverter<ObjectId>
{
    /// <summary>
    /// Reads a JSON string and converts it to an ObjectId instance.
    /// </summary>
    public override ObjectId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            var root = doc.RootElement;
            var json = root.ToString();

            if (ObjectId.TryParse(json, out var id))
            {
                return id;
            }

            return new ObjectId();
        }
    }

    /// <summary>
    /// Writes an ObjectId value as a JSON string.
    /// </summary>
    public override void Write(Utf8JsonWriter writer, ObjectId value, JsonSerializerOptions options)
    {
        writer.WriteRawValue($"\"{value}\"");
    }
}

public class NewtonsoftObjectIdConverter : Newtonsoft.Json.JsonConverter<ObjectId>
{
    /// <summary>
    /// Reads a JSON string and converts it to an ObjectId instance.
    /// </summary>
    public override ObjectId ReadJson(Newtonsoft.Json.JsonReader reader, Type objectType, ObjectId existingValue, bool hasExistingValue, Newtonsoft.Json.JsonSerializer serializer)
    {
        var json = reader.Value?.ToString();

        if (json != null && ObjectId.TryParse(json, out var id))
        {
            return id;
        }

        return new ObjectId();
    }

    /// <summary>
    /// Writes an ObjectId value as a JSON string.
    /// </summary>
    public override void WriteJson(Newtonsoft.Json.JsonWriter writer, ObjectId value, Newtonsoft.Json.JsonSerializer serializer)
    {
        writer.WriteValue(value.ToString());
    }
}
