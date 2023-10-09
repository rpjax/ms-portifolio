namespace ModularSystem.Core;

/// <summary>
/// Represents a contract for serializing and deserializing objects.
/// </summary>
public interface ISerializer
{
    /// <summary>
    /// Serializes the specified object into its string representation.
    /// </summary>
    /// <typeparam name="T">The type of the object to serialize.</typeparam>
    /// <param name="obj">The object to serialize.</param>
    /// <returns>A string representation of the serialized object.</returns>
    string Serialize<T>(T obj);

    /// <summary>
    /// Attempts to deserialize the provided string back into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="serialized">The string representation of the object.</param>
    /// <returns>An instance of type <typeparamref name="T"/> if successful, otherwise <c>null</c>.</returns>
    T? TryDeserialize<T>(string serialized);

    /// <summary>
    /// Deserializes the provided string back into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the object to deserialize to.</typeparam>
    /// <param name="serialized">The string representation of the object.</param>
    /// <returns>An instance of type <typeparamref name="T"/>.</returns>
    /// <exception cref="System.Exception">Thrown if the deserialization process encounters an error.</exception>
    T Deserialize<T>(string serialized);
}

/// <summary>
/// Defines a serialization and deserialization contract for objects of type <typeparamref name="T"/>.
/// </summary>
/// <typeparam name="T">The type of object to be serialized or deserialized.</typeparam>
public interface ISerializer<T>
{
    /// <summary>
    /// Serializes the specified object of type <typeparamref name="T"/> into a string representation.
    /// </summary>
    /// <param name="obj">The object of type <typeparamref name="T"/> to serialize.</param>
    /// <returns>A string representation of the serialized object.</returns>
    string Serialize(T obj);

    /// <summary>
    /// Attempts to deserialize the given string representation into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="serialized">The string representation of the object to be deserialized.</param>
    /// <returns>An instance of <typeparamref name="T"/> if deserialization is successful; otherwise, null.</returns>
    /// <remarks>
    /// This method is safe to use when you're unsure if the string representation is a valid serialization of type <typeparamref name="T"/>,
    /// as it won't throw an exception for invalid inputs but will return null instead.
    /// </remarks>
    T? TryDeserialize(string serialized);

    /// <summary>
    /// Deserializes the given string representation into an object of type <typeparamref name="T"/>.
    /// </summary>
    /// <param name="serialized">The string representation of the object to be deserialized.</param>
    /// <returns>An instance of <typeparamref name="T"/>.</returns>
    /// <exception cref="System.InvalidOperationException">Thrown when the provided string cannot be deserialized into an instance of <typeparamref name="T"/>.</exception>
    /// <remarks>
    /// Use this method when you're confident that the string representation is a valid serialization of type <typeparamref name="T"/>,
    /// as invalid inputs can result in exceptions.
    /// </remarks>
    T Deserialize(string serialized);
}
