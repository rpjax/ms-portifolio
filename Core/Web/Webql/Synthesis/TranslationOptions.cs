using ModularSystem.Core;
using ModularSystem.Mongo;
using MongoDB.Bson;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Contains options for translating WebQL nodes into LINQ expressions.
/// </summary>
public class TranslationOptions
{
    /// <summary>
    /// Gets or sets the LINQ provider used for translating nodes.
    /// </summary>
    public ILinqProvider LinqProvider { get; set; } = Synthesis.LinqProvider.Queryable;

    public JsonSerializerOptions? SerializerOptions { get; set; } = null;

    public Dictionary<string, TypeInformation> TypesDictionary { get; set; } = new();

    /// <summary>
    /// Indicates whether the 'Take' operation supports Int64.
    /// </summary>
    public bool TakeSupportsInt64 { get; set; } = false;

    /// <summary>
    /// Indicates whether the 'Skip' operation supports Int64.
    /// </summary>
    public bool SkipSupportsInt64 { get; set; } = false;

    /// <summary>
    /// Initializes a new instance of the TranslatorOptions class.
    /// </summary>
    public TranslationOptions()
    {
       AddDefaultTypes();
    }

    public Type? TryGetType(string name)
    {
        if(TypesDictionary.TryGetValue(name, out var type))
        {
            return type.Type;
        }

        return null;
    }

    public object? Deserialize(string json, Type type)
    {
        return JsonSerializer.Deserialize(json, type);
    }

    void AddDefaultTypes()
    {

    }

}

public class TypeInformation : ObjectIdJsonConverter
{
    public string Name { get; }
    public Type Type { get; }
    public IStrategy<string, object> DeserializationStrategy { get; }
    public JsonConverter<object> JsonConverter { get; }

    public TypeInformation(string name, Type type, IStrategy<string, object> deserializationStrategy)
    {
        Name = name;
        Type = type;
        DeserializationStrategy = deserializationStrategy;
    }

    public JsonConverter<object> GetJsonConverter()
    {
        return new Converter(DeserializationStrategy);
    }

    class Converter : JsonConverter<object>
    {
        public IStrategy<string, object> DeserializationStrategy { get; }

        public Converter(IStrategy<string, object> deserializationStrategy)
        {
            DeserializationStrategy = deserializationStrategy;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using var doc = JsonDocument.ParseValue(ref reader);

            var root = doc.RootElement;
            var json = root.ToString();

            return DeserializationStrategy.Execute(json);
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

}
