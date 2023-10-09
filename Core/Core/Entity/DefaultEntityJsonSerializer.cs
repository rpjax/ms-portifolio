using System.Text.Json;

namespace ModularSystem.Core;

/// <summary>
/// Default JSON serializer for entities of type T.
/// </summary>
/// <typeparam name="T">The type of the entity to be serialized, which must be a class.</typeparam>
public class DefaultEntityJsonSerializer<T> : ISerializer<T> where T : class
{
    public string Serialize(T input)
    {
        return JsonSerializer.Serialize(input);
    }

    public T? TryDeserialize(string input)
    {
        var options = new JsonSerializerOptions();
        var obj = JsonSerializer.Deserialize<T>(input, options);

        if (obj == null)
        {
            return null;
        }

        return obj;
    }

    public T Deserialize(string input)
    {
        var obj = TryDeserialize(input);

        if (obj == null)
        {
            throw new AppException($"Could not deserialize the given JSON to an instance of type: {typeof(T).FullName}.");
        }

        return obj;
    }
}