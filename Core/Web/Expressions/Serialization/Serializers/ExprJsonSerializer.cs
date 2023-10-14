using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// A specialized JSON serializer designed for <see cref="ExprSerializer"/>, emphasizing the handling of 
/// the abstract <see cref="SerializableExpression"/>.
/// </summary>
/// <remarks>
/// Given the abstract nature of <see cref="SerializableExpression"/>, this serializer incorporates <br/>
/// custom logic to discern
/// the appropriate concrete type during deserialization from a serialized string.
/// </remarks>
public class ExprJsonSerializer : ISerializer
{
    /// <summary>
    /// Gets the JSON serialization options used by this serializer.
    /// </summary>
    private JsonSerializerOptions? Options { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ExprJsonSerializer"/> class.
    /// </summary>
    /// <param name="options">The JSON serialization options to be used by this serializer.</param>
    public ExprJsonSerializer(JsonSerializerOptions? options = null)
    {
        Options = options;
    }

    /// <summary>
    /// Serializes the provided object into a JSON string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to be serialized.</param>
    /// <returns>The JSON string representation of the serialized object.</returns>
    public virtual string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, GetOptions());
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="serialized">The JSON string representation of the object to be deserialized.</param>
    /// <returns>The deserialized object if successful; otherwise, <c>null</c>.</returns>
    public virtual T? TryDeserialize<T>(string serialized)
    {
        return JsonSerializer.Deserialize<T>(serialized, GetOptions());
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into an object of the specified type.
    /// </summary>
    /// <param name="serialized">The JSON string representation of the object to be deserialized.</param>
    /// <param name="type">The type of the object to deserialize to.</param>
    /// <returns>The deserialized object if successful; otherwise, <c>null</c>.</returns>
    public virtual object? TryDeserialize(string serialized, Type type)
    {
        return JsonSerializer.Deserialize(serialized, type, GetOptions());
    }

    /// <summary>
    /// Deserializes a JSON string into an object of the specified type.
    /// </summary>
    /// <param name="serialized">The JSON string representation of the object to be deserialized.</param>
    /// <param name="type">The type of the object to deserialize to.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="System.Exception">Thrown if the deserialization process encounters an error or if the result is null.</exception>
    public object Deserialize(string serialized, Type type)
    {
        var deserialized = TryDeserialize(serialized, type);

        if (deserialized == null)
        {
            throw new Exception("Could not deserialize the provided string into an instance of SerializableExpression");
        }

        return deserialized;
    }

    /// <summary>
    /// Deserializes a JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="serialized">The JSON string representation of the object to be deserialized.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="System.Exception">Thrown if the deserialization process encounters an error.</exception>
    public virtual T Deserialize<T>(string serialized)
    {
        return Deserialize(serialized, typeof(T)).TypeCast<T>();
    }

    /// <summary>
    /// Retrieves the JSON serialization options, including the custom converter for <see cref="SerializableExpression"/>.
    /// </summary>
    /// <returns>The configured <see cref="JsonSerializerOptions"/>.</returns>
    protected virtual JsonSerializerOptions GetOptions()
    {
        var options = JsonSerializerSingleton.GetOptions(Options);
        options.AddConverter<ExpressionJsonConverter>();
        return options;
    }
}

internal class ExpressionJsonConverter : JsonConverter<SerializableExpression>
{
    /// <summary>
    /// Reads a JSON string and converts it to an ObjectId instance.
    /// </summary>
    public override SerializableExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var root = jsonDocument.RootElement;
        var json = root.ToString();

        if (root.TryGetProperty(nameof(SerializableExpression.NodeType), out var nodeTypeProperty))
        {
            throw new Exception($"Could not deserialize the provided JSON string to an instance of SerializableNode. The provided JSON does not contain a '{nameof(SerializableExpression.NodeType)}' property.");
        }

        if (nodeTypeProperty.ValueKind != JsonValueKind.Number)
        {
            throw new Exception("The 'NodeType' property in the provided JSON string is not a valid number. Ensure the JSON represents a valid SerializableExpression.");
        }

        var nodeTypeInt = nodeTypeProperty.GetInt32();
        var nodeType = (ExtendedExpressionType)nodeTypeInt;
        var emptynode = new EmptySerializableNode() as SerializableExpression;
        var concreteType = SerializableExpression.GetConcreteType(emptynode.NodeType);
        var node = root.Deserialize(concreteType, options)?.TryTypeCast<SerializableExpression>();

        if (node == null)
        {
            throw new Exception($"Could not deserialize the provided JSON string to an instance of SerializableNode of type'{concreteType.FullName}'.");
        }

        return node;
    }

    public override void Write(Utf8JsonWriter writer, SerializableExpression value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, SerializableExpression.GetConcreteType(value.NodeType), options);
    }
}
