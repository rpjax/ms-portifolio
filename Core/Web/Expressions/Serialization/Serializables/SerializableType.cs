using ModularSystem.Core;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Serialized version of <see cref="Type"/>
/// </summary>
public class SerializableType
{
    public bool IsGeneric { get; set; }
    public string? AssemblyQualifiedName { get; set; }
    public string? Namespace { get; set; }
    public string? Name { get; set; }
    public string? FullName => GetFullName();
    public SerializableType[] GenericTypeArguments { get; set; } = Array.Empty<SerializableType>();

    public string GetFullName()
    {
        if (FullNameIsAvailable())
        {
            return $"{Namespace}.{Name}";
        }
        else if (string.IsNullOrEmpty(Namespace) && !string.IsNullOrEmpty(Name))
        {
            return Name;
        }
        else
        {
            return string.Empty;
        }
    }

    public bool FullNameIsAvailable()
    {
        return !string.IsNullOrEmpty(Namespace) && !string.IsNullOrEmpty(Name);
    }

    public bool AssemblyNameIsAvailable()
    {
        return !string.IsNullOrEmpty(AssemblyQualifiedName);
    }

    public bool ContainsGenericArguments()
    {
        return GenericTypeArguments.IsNotEmpty();
    }
}
