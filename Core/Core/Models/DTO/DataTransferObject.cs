using System.Text.Json.Serialization;

namespace ModularSystem.Core;

/// <summary>
/// Represents a Data Transfer Object (DTO) that encapsulates a single value of type <typeparamref name="T"/>.<br/>
/// This can be used to wrap values for standardized data transfer processes.
/// </summary>
/// <typeparam name="T">The type of value encapsulated by this DTO.</typeparam>
public class Dto<T>
{
    [JsonPropertyName("value")]
    public T? Value { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dto{T}"/> class.
    /// </summary>
    public Dto() : this(default)
    {

    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Dto{T}"/> class with the provided value.
    /// </summary>
    /// <param name="value">The value to encapsulate.</param>
    public Dto(T? value)
    {
        Value = value;
    }

    /// <summary>
    /// Creates a new DTO from the provided value.
    /// </summary>
    /// <param name="value">The value to encapsulate.</param>
    /// <returns>A new DTO containing the provided value.</returns>
    public static Dto<T> From(T? value)
    {
        return new Dto<T>(value);
    }

    public override bool Equals(object? obj)
    {
        if (obj is Dto<T> otherDto)
        {
            return EqualityComparer<T>.Default.Equals(Value, otherDto.Value);
        }
        return false;
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }
}
