using ModularSystem.Core;
using System.Text.Json;

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
        options.AddConverter<ExprJsonConverter>();
        return options;
    }
}
