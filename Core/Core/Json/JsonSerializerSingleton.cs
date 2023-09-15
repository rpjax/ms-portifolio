using System.Collections.Concurrent;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Provides a centralized JSON serialization and deserialization facility with a static set of converters.<br/>
/// This singleton approach ensures consistent JSON processing throughout the application.
/// </summary>
public static class JsonSerializerSingleton
{
    /// <summary>
    /// Gets the static collection of JSON converters to be used by the <see cref="JsonSerializer"/>.
    /// </summary>
    private static ConcurrentDictionary<string, JsonConverter> Converters { get; }

    /// <summary>
    /// Static constructor to initialize the static Converters list.
    /// </summary>
    static JsonSerializerSingleton()
    {
        Converters = new();
    }

    public static bool TryAddConverter(string key, JsonConverter converter)
    {
        if (!Converters.ContainsKey(key))
        {
            return Converters.TryAdd(key, converter);
        }

        return false;
    }

    public static bool TryAddConverter(Type type, JsonConverter converter)
    {
        return TryAddConverter(type.FullName!, converter);
    }

    public static IEnumerable<JsonConverter> GetConverters()
    {
        return Converters.Values;
    }

    /// <summary>
    /// Serializes the provided value into a JSON string using the specified options.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">The serialization options.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string Serialize<T>(T value, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Serialize(value, GetOptions(options));
    }

    /// <summary>
    /// Deserializes the provided JSON document into an object of type T using the specified options.
    /// </summary>
    /// <param name="document">The JSON document to deserialize.</param>
    /// <param name="options">The deserialization options. Defaults to null.</param>
    /// <returns>An object of type T.</returns>
    public static T? Deserialize<T>(JsonDocument document, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(document, GetOptions(options));
    }

    /// <summary>
    /// Deserializes the provided JSON string into an object of type T using the specified options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">The deserialization options. Defaults to null.</param>
    /// <returns>An object of type T.</returns>
    public static object? Deserialize(string json, Type type, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize(json, type, GetOptions(options));
    }

    /// <summary>
    /// Deserializes the provided JSON string into an object of type T using the specified options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">The deserialization options. Defaults to null.</param>
    /// <returns>An object of type T.</returns>
    public static ValueTask<object?> DeserializeAsync(Stream json, Type type, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.DeserializeAsync(json, type, GetOptions(options));
    }

    /// <summary>
    /// Deserializes the provided JSON string into an object of type T using the specified options.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">The deserialization options. Defaults to null.</param>
    /// <returns>An object of type T.</returns>
    public static T? Deserialize<T>(string json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.Deserialize<T>(json, GetOptions(options));
    }

    /// <summary>
    /// Deserializes the provided JSON string into an object of type T using the specified options.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">The deserialization options. Defaults to null.</param>
    /// <returns>An object of type T.</returns>
    public static ValueTask<T?> DeserializeAsync<T>(Stream json, JsonSerializerOptions? options = null)
    {
        return JsonSerializer.DeserializeAsync<T>(json, GetOptions(options));
    }

    /// <summary>
    /// Retrieves or creates a <see cref="JsonSerializerOptions"/> instance, and injects the static
    /// <see cref="JsonConverter"/> items added from this container.
    /// </summary>
    /// <param name="options">The existing options to augment with the static converters. If null and <paramref name="create_if_null"/> is true, a new instance will be created.</param>
    /// <param name="create_if_null">Specifies whether a new JsonSerializerOptions instance should be created if the provided one is null. Defaults to true.</param>
    /// <returns>The augmented or created JsonSerializerOptions.</returns>
    public static JsonSerializerOptions GetOptions(JsonSerializerOptions? options = null, bool create_if_null = true)
    {
        if (create_if_null)
        {
            options ??= new JsonSerializerOptions();
        }

        if (options != null)
        {
            foreach (var converter in Converters.Values)
            {
                if (!options.Converters.Contains(converter))
                {
                    options.Converters.Add(converter);
                }
            }
        }

        return options;
    }
}
