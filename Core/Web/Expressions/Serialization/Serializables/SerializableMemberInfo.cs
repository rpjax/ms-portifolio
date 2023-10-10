using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Serialized version of <see cref="MemberInfo"/>
/// </summary>
public class SerializableMemberInfo
{
    public SerializableType? DeclaringType { get; set; }
    public string? Name { get; set; }
}
