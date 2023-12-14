using ModularSystem.Core;
using System.Text;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Represents a serializable version of the .NET <see cref="Type"/> class. <br/>
/// This class is designed to capture the essential information about a type in a format that can be easily serialized and deserialized.
/// </summary>
public class SerializableType
{

    public bool IsGenericMethodParameter { get; set; }
    public bool IsGenericParameter { get; set; }
    public bool IsGenericType { get; set; }
    public bool IsGenericTypeParameter { get; set; }
    public bool IsGenericTypeDefinition { get; set; }

    /// <summary>
    /// Gets or sets the assembly-qualified name of the type.
    /// </summary>
    public string? AssemblyQualifiedName { get; set; }

    /// <summary>
    /// Gets or sets the namespace of the type.
    /// </summary>
    public string? Namespace { get; set; }

    /// <summary>
    /// Gets or sets the simple name of the type.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets the full name of the type, which is a combination of its namespace and its simple name.
    /// </summary>
    public string? FullName => GetFullName();

    /// <summary>
    /// Gets or sets the qualified full name of the type, including its generic type arguments if applicable.
    /// </summary>
    public string? QualifiedFullName => GetQualifiedFullName();

    public SerializableType? GenericTypeDefinition { get; set; }

    /// <summary>
    /// Gets or sets the array of generic type arguments if the type is a generic type.
    /// </summary>
    public SerializableType[] GenericTypeArguments { get; set; } = Array.Empty<SerializableType>();

    /// <summary>
    /// Computes and returns the full name of the type.
    /// </summary>
    /// <returns>The full name of the type.</returns>
    public string? GetFullName()
    {
        if (Namespace != null && Name != null)
        {
            return $"{Namespace}.{Name}";
        }
        else if (Name != null)
        {
            return Name;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// Determines whether the full name of the type is available.
    /// </summary>
    /// <returns><c>true</c> if the full name is available; otherwise, <c>false</c>.</returns>
    public bool FullNameIsAvailable()
    {
        return !string.IsNullOrEmpty(Namespace) && !string.IsNullOrEmpty(Name);
    }

    /// <summary>
    /// Determines whether the assembly name of the type is available.
    /// </summary>
    /// <returns><c>true</c> if the assembly name is available; otherwise, <c>false</c>.</returns>
    public bool AssemblyNameIsAvailable()
    {
        return !string.IsNullOrEmpty(AssemblyQualifiedName);
    }

    /// <summary>
    /// Determines whether the type has generic type arguments.
    /// </summary>
    /// <returns><c>true</c> if the type has generic type arguments; otherwise, <c>false</c>.</returns>
    public bool ContainsGenericArguments()
    {
        return GenericTypeArguments.IsNotEmpty();
    }

    /// <summary>
    /// Computes and returns the qualified full name of the type, including its generic type arguments if applicable.
    /// </summary>
    /// <returns>The qualified full name of the type.</returns>
    /// <exception cref="ArgumentException">Thrown if the full name is not in the expected format or if there's a mismatch between the number of generic type arguments and the expected count.</exception>
    public string GetQualifiedFullName()
    {
        if (FullName == null)
        {
            throw new ArgumentException(nameof(FullName));
        }

        if (!IsGenericType)
        {
            return FullName;
        }

        var split = FullName.Split('`');

        if (split.Length != 2)
        {
            throw new ArgumentException(FullName);
        }

        var fullname = split[0];
        var argCountStr = split[1];

        if (!int.TryParse(argCountStr, out var argCount))
        {
            throw new ArgumentException(FullName);
        }
        if (argCount != GenericTypeArguments.Length)
        {
            throw new ArgumentException(FullName);
        }

        var argNames = GenericTypeArguments.Select(x => x.FullName).ToArray();
        var strBuilder = new StringBuilder();

        strBuilder.Append($"{fullname}`{argCount}[");

        for (int i = 0; i < argNames.Length; i++)
        {
            var argName = argNames[i];
            var isLast = argNames.Length - 1 == i;

            if (argName == null)
            {
                throw new ArgumentException(FullName);
            }
            if (isLast)
            {
                strBuilder.Append($"[{argName}]");
            }
            else
            {
                strBuilder.Append($"[{argName}],");
            }
        }

        strBuilder.Append(']');
        return strBuilder.ToString();
    }
}
