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
    /// Specifies the strategy to use when there's a conflict while adding a JSON converter.
    /// </summary>
    public enum ConverterConflictStrategy
    {
        /// <summary>
        /// Retains the existing converter and ignores the new one.
        /// </summary>
        UseOldest,

        /// <summary>
        /// Replaces the existing converter with the new one.
        /// </summary>
        UseNewest,

        /// <summary>
        /// Throws an exception if a converter of the same type already exists.
        /// </summary>
        Throw
    }

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

    /// <summary>
    /// Attempts to add a new JSON converter to the static collection. <br/>
    /// If a converter of the same type already exists, 
    /// the behavior is determined by the provided conflict strategy.
    /// </summary>
    /// <typeparam name="T">The type of the JSON converter.</typeparam>
    /// <param name="converter">The converter instance to add.</param>
    /// <param name="strategy">The strategy to use if a converter of the same type already exists.</param>
    /// <returns>True if the converter was added successfully; otherwise, false.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the conflict strategy is set to 'Throw' and a converter of the same type already exists.</exception>
    public static bool TryAddConverter<T>(T converter, ConverterConflictStrategy strategy = ConverterConflictStrategy.Throw) where T : JsonConverter
    {
        var type = typeof(T);
        var key = type.FullName ?? type.Name;

        if (!Converters.ContainsKey(key))
        {
            return Converters.TryAdd(key, converter);
        }
        if(strategy == ConverterConflictStrategy.UseOldest)
        {
            return false;
        }
        if(strategy == ConverterConflictStrategy.UseNewest)
        {
            Converters.Remove(key, out _);
            return Converters.TryAdd(key, converter);
        }
        if (strategy == ConverterConflictStrategy.Throw)
        {
            throw new InvalidOperationException($"Conflict detected: A JSON converter of type '{key}' is already registered. To resolve this, consider using a different conflict strategy.");
        }

        return false;
    }

    /// <summary>
    /// Retrieves all the JSON converters currently registered in the static collection.
    /// </summary>
    /// <returns>An enumerable of registered JSON converters.</returns>
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

    public static void Serialize<T>(Utf8JsonWriter writer, T value, JsonSerializerOptions? options = null)
    {
        JsonSerializer.Serialize(writer, value, GetOptions(options));
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
    /// <returns>The augmented or created JsonSerializerOptions.</returns>
    public static JsonSerializerOptions GetOptions(JsonSerializerOptions? options = null)
    {
        options ??= new JsonSerializerOptions();

        foreach (var converter in Converters.Values)
        {
            if (!options.Converters.Contains(converter))
            {
                options.Converters.Add(converter);
            }
        }

        return options;
    }
}
