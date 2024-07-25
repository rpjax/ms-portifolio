using ModularSystem.Core;
using System.Text.Json.Serialization;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a serializable representation of the .NET <see cref="Type"/> class, suitable for JSON serialization and deserialization. <br/>
/// This class encapsulates key characteristics of a type, such as its genericity, namespace, and name, allowing for easy and robust type information sharing across different systems.
/// </summary>
public class SerializableType
{
    /// <summary>
    /// Indicates if the type is a generic method parameter.
    /// </summary>
    public bool IsGenericMethodParameter { get; set; }

    /// <summary>
    /// Indicates if the type is a generic type parameter.
    /// </summary>
    public bool IsGenericParameter { get; set; }

    /// <summary>
    /// Indicates if the type is a generic type.
    /// </summary>
    public bool IsGenericType { get; set; }

    /// <summary>
    /// Indicates if the type is a generic type parameter.
    /// </summary>
    public bool IsGenericTypeParameter { get; set; }

    /// <summary>
    /// Indicates if the type is a definition of a generic type.
    /// </summary>
    public bool IsGenericTypeDefinition { get; set; }

    /// <summary>
    /// Indicates if the type is an anonymous type.
    /// </summary>
    public bool IsAnonymousType { get; set; }

    /// <summary>
    /// Contains the definitions of properties for an anonymous type.
    /// </summary>
    public SerializablePropertyDefinition[] AnonymousPropertyDefinitions { get; set; } = Array.Empty<SerializablePropertyDefinition>();

    /// <summary>
    /// The assembly-qualified name of the type.
    /// </summary>
    public string? AssemblyQualifiedName { get; set; }

    /// <summary>
    /// The namespace of the type.
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// The simple name of the type.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The generic type definition of the type, if applicable.
    /// </summary>
    public SerializableType? GenericTypeDefinition { get; set; }

    /// <summary>
    /// The array of generic type arguments of the type, if it is a generic type.
    /// </summary>
    public SerializableType[] GenericTypeArguments { get; set; } = Array.Empty<SerializableType>();

    /// <summary>
    /// The full name of the type, combining its namespace and simple name.
    /// </summary>
    public string? FullName => GetFullName();

    /// <summary>
    /// Retrieves the full name of the type, combining the namespace and the name.
    /// </summary>
    /// <returns>The full name of the type, or null if it cannot be determined.</returns>
    public string? GetFullName()
    {
        return Namespace != null && Name != null ? $"{Namespace}.{Name}" : Name;
    }

    /// <summary>
    /// Checks if the full name of the type is available.
    /// </summary>
    /// <returns>True if the full name is available; otherwise, false.</returns>
    public bool FullNameIsAvailable()
    {
        return !string.IsNullOrEmpty(Namespace) && !string.IsNullOrEmpty(Name);
    }

    /// <summary>
    /// Checks if the assembly name of the type is available.
    /// </summary>
    /// <returns>True if the assembly name is available; otherwise, false.</returns>
    public bool AssemblyNameIsAvailable()
    {
        return !string.IsNullOrEmpty(AssemblyQualifiedName);
    }

    /// <summary>
    /// Checks if the type has generic type arguments.
    /// </summary>
    /// <returns>True if the type has generic type arguments; otherwise, false.</returns>
    public bool ContainsGenericArguments()
    {
        return GenericTypeArguments.IsNotEmpty();
    }
}

/// <summary>
/// Represents a definition of a property in a serializable type, encapsulating the property's name and type.
/// </summary>
public class SerializablePropertyDefinition
{
    /// <summary>
    /// The name of the property.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// The serializable type of the property.
    /// </summary>
    public SerializableType? Type { get; set; }

    /// <summary>
    /// Initializes a new instance of <see cref="SerializablePropertyDefinition"/> for JSON serialization.
    /// </summary>
    [JsonConstructor]
    public SerializablePropertyDefinition()
    {
    }

    /// <summary>
    /// Initializes a new instance of <see cref="SerializablePropertyDefinition"/> with the specified name and type.
    /// </summary>
    /// <param name="name">The name of the property.</param>
    /// <param name="type">The serializable type of the property.</param>
    public SerializablePropertyDefinition(string? name, SerializableType? type)
    {
        Name = name;
        Type = type;
    }
}
