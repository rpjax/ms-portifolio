using ModularSystem.Core;
using ModularSystem.Core.Expressions;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Web.Expressions;

internal class ExprJsonConverter : JsonConverter<SerializableExpression>
{
    /// <summary>
    /// Reads a JSON string and converts it to an ObjectId instance.
    /// </summary>
    public override SerializableExpression Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var root = jsonDocument.RootElement;
        var json = root.ToString();

        if (!root.TryGetProperty(nameof(SerializableExpression.NodeType), out var nodeTypeProperty))
        {
            throw new AppException($"Could not deserialize the provided JSON string to an instance of SerializableNode. The provided JSON does not contain a '{nameof(SerializableExpression.NodeType)}' property.", ExceptionCode.InvalidInput);
        }

        if (nodeTypeProperty.ValueKind != JsonValueKind.Number)
        {
            throw new AppException("The 'NodeType' property in the provided JSON string is not a valid number. Ensure the JSON represents a valid SerializableExpression.", ExceptionCode.InvalidInput);
        }

        var nodeTypeInt = nodeTypeProperty.GetInt32();
        var nodeType = (ExtendedExpressionType)nodeTypeInt;
        var concreteType = SerializableExpression.GetConcreteType(nodeType);
        var node = root.Deserialize(concreteType, options)?.TryTypeCast<SerializableExpression>();

        if (node == null)
        {
            throw new AppException($"Could not deserialize the provided JSON string to an instance of SerializableExpression of type'{concreteType.FullName}'.", ExceptionCode.InvalidInput);
        }

        return node;
    }

    public override void Write(Utf8JsonWriter writer, SerializableExpression value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, SerializableExpression.GetConcreteType(value.NodeType), options);
    }
}
