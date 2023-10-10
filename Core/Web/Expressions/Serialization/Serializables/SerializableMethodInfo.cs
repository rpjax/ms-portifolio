using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

public class SerializableMethodInfo
{
    public bool IsGenericMethod { get; set; }
    public string? Name { get; set; }
    public BindingFlags BindingFlags { get; set; }
    public SerializableType? DeclaringType { get; set; }
    public SerializableType? ReturnType { get; set; }
    public SerializableParameterInfo[] Parameters { get; set; }    
    public SerializableType[] GenericArguments { get; set; } = Array.Empty<SerializableType>();

    /// <summary>
    /// Type.Fullname + Name
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GetFullName()
    {
        if (DeclaringType == null)
        {
            throw new InvalidOperationException();
        }
        if (Name == null)
        {
            throw new InvalidOperationException();
        }

        return $"{DeclaringType.GetFullName()}.{Name}";
    }
}
