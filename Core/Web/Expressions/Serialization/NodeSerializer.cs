using ModularSystem.Core;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// A specialized JSON serializer designed for <see cref="ExprSerializer"/>, emphasizing the handling of 
/// the abstract <see cref="SerializableNode"/>.
/// </summary>
/// <remarks>
/// Given the abstract nature of <see cref="SerializableNode"/>, this serializer incorporates custom logic to discern
/// <br/> the appropriate concrete type during deserialization from a serialized string.
/// </remarks>
public class NodeSerializer : ISerializer
{
    /// <summary>
    /// Serializes the provided object into a JSON string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to be serialized.</param>
    /// <returns>The JSON string representation of the serialized object.</returns>
    public virtual string Serialize<T>(T obj)
    {
        return JsonSerializer.Serialize(obj, Options());
    }

    /// <summary>
    /// Attempts to deserialize a JSON string into an object of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="serialized">The JSON string representation of the object to be deserialized.</param>
    /// <returns>The deserialized object if successful; otherwise, <c>null</c>.</returns>
    public virtual T? TryDeserialize<T>(string serialized)
    {
        return JsonSerializer.Deserialize<T>(serialized, Options());
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
        var deserialized = TryDeserialize<T>(serialized);

        if (deserialized == null)
        {
            throw new Exception("Could not deserialize the provided string into an instance of SerializableExpression");
        }

        return deserialized;
    }

    /// <summary>
    /// Retrieves the JSON serialization options, including the custom converter for <see cref="SerializableNode"/>.
    /// </summary>
    /// <returns>The configured <see cref="JsonSerializerOptions"/>.</returns>
    protected virtual JsonSerializerOptions Options()
    {
        return JsonSerializerSingleton.GetOptions(new()
        {
            Converters = { new NodeJsonConverter() }
        });
    }
}

internal class NodeJsonConverter : JsonConverter<SerializableNode>
{
    /// <summary>
    /// Reads a JSON string and converts it to an ObjectId instance.
    /// </summary>
    public override SerializableNode Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var root = jsonDocument.RootElement;
        var json = root.ToString();

        if (root.TryGetProperty(nameof(SerializableNode.NodeType), out var nodeTypeProperty))
        {
            throw new Exception($"Could not deserialize the provided JSON string to an instance of SerializableNode. The provided JSON does not contain a '{nameof(SerializableNode.NodeType)}' property.");
        }

        var nodeTypeInt = nodeTypeProperty.GetInt32();
        var nodeType = (ExtendedExpressionType)nodeTypeInt;
        var emptynode = new EmptySerializableNode() as SerializableNode;
        var concreteType = SerializableNode.GetConcreteType(emptynode.NodeType);
        var node = root.Deserialize(concreteType, options)?.TryTypeCast<SerializableNode>();

        if(node == null)
        {
            throw new Exception($"Could not deserialize the provided JSON string to an instance of SerializableNode of type'{concreteType.FullName}'.");
        }

        return node;  
    }

    public override void Write(Utf8JsonWriter writer, SerializableNode value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, SerializableNode.GetConcreteType(value.NodeType), JsonSerializerSingleton.GetOptions());
    }
}
