using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public class SerializableMemberBinding
{
    public MemberBindingType BindingType { get; set; }
    public SerializableExpression? Expression { get; set; }
    public SerializableMemberInfo? MemberInfo { get; set; }
    public SerializableMemberBinding[] Bindings { get; set; } = Array.Empty<SerializableMemberBinding>();
    public SerializableElementInit[] Initializers { get; set; } = Array.Empty<SerializableElementInit>();
}
