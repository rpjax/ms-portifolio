namespace ModularSystem.Core;

/// <summary>
/// Defines the contract for serialization and deserialization operations.
/// </summary>
public interface ISerializer
{
    /// <summary>
    /// Serializes the provided object into a string representation.
    /// </summary>
    /// <param name="obj">The object to be serialized.</param>
    /// <returns>The string representation of the serialized object.</returns>
    string Serialize(object obj);

    /// <summary>
    /// Attempts to deserialize a string into an object. If the deserialization fails, 
    /// it returns <c>null</c> instead of throwing an exception.
    /// </summary>
    /// <param name="serialized">The string representation of the object to be deserialized.</param>
    /// <returns>The deserialized object if the operation is successful; otherwise, <c>null</c>.</returns>
    object? TryDeserialize(string serialized);

    /// <summary>
    /// Deserializes a string into an object. If the deserialization fails, an exception is thrown.
    /// </summary>
    /// <param name="serialized">The string representation of the object to be deserialized.</param>
    /// <returns>The deserialized object.</returns>
    /// <exception cref="System.Exception">Thrown when the deserialization process encounters an error.</exception>
    object Deserialize(string serialized);
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
