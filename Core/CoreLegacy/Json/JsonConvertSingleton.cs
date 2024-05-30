using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace ModularSystem.Core;

public static class JsonConvertSingleton
{
    /// <summary>
    /// Gets the static collection of JSON converters to be used by the <see cref="JsonConvert"/>.
    /// </summary>
    private static ConcurrentDictionary<string, JsonConverter> Converters { get; }

    /// <summary>
    /// Static constructor to initialize the static Converters list.
    /// </summary>
    static JsonConvertSingleton()
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

    public static JsonSerializerSettings GetOptions(JsonSerializerSettings? options = null)
    {
        options ??= new JsonSerializerSettings();

        return options;
    }


    /// <summary>
    /// Serializes the provided value into a JSON string using the specified options.
    /// </summary>
    /// <param name="value">The object to serialize.</param>
    /// <param name="options">The serialization options.</param>
    /// <returns>A JSON string representation of the object.</returns>
    public static string SerializeObject<T>(T value, JsonSerializerSettings? options = null)
    {
        return JsonConvert.SerializeObject(value, GetOptions(options));
    }


    /// <summary>
    /// Deserializes the provided JSON string into an object of type T using the specified options.
    /// </summary>
    /// <param name="type"></param>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">The deserialization options. Defaults to null.</param>
    /// <returns>An object of type T.</returns>
    public static object? Deserialize(string json, Type type, JsonSerializerSettings? options = null)
    {
        return JsonConvert.DeserializeObject(json, type, GetOptions(options));
    }

    /// <summary>
    /// Deserializes the provided JSON string into an object of type T using the specified options.
    /// </summary>
    /// <param name="json">The JSON string to deserialize.</param>
    /// <param name="options">The deserialization options. Defaults to null.</param>
    /// <returns>An object of type T.</returns>
    public static T? Deserialize<T>(string json, JsonSerializerSettings? options = null)
    {
        return JsonConvert.DeserializeObject<T>(json, GetOptions(options));
    }
}