using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents a generic key-value pair. <br/>
/// This class provides a flexible structure to store two related objects - a key and its corresponding value.
/// </summary>
/// <typeparam name="TKey">The type of the key in the key-value pair.</typeparam>
/// <typeparam name="TValue">The type of the value in the key-value pair.</typeparam>
public class KeyValue<TKey, TValue>
{
    /// <summary>
    /// Gets or sets the key element of the key-value pair.
    /// </summary>
    /// <remarks>
    /// The key can be of any type specified by the <typeparamref name="TKey"/> parameter.
    /// </remarks>
    public TKey? Key { get; set; } = default;

    /// <summary>
    /// Gets or sets the value element of the key-value pair.
    /// </summary>
    /// <remarks>
    /// The value can be of any type specified by the <typeparamref name="TValue"/> parameter.
    /// </remarks>
    public TValue? Value { get; set; } = default;

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyValue{TKey, TValue}"/> class.
    /// </summary>
    /// <remarks>
    /// This parameterless constructor is used during deserialization.
    /// </remarks>
    [JsonConstructor]
    public KeyValue()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="KeyValue{TKey, TValue}"/> class with the specified key and value.
    /// </summary>
    /// <param name="key">The key of the key-value pair.</param>
    /// <param name="value">The value of the key-value pair.</param>
    public KeyValue(TKey? key, TValue? value)
    {
        Key = key;
        Value = value;
    }
}
