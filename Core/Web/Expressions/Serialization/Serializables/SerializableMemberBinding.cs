namespace ModularSystem.Web.Expressions;

public class SerializableMemberBinding
{
    public SerializableMemberInfo? MemberInfo { get; set; }
    public SerializableMemberBinding[] Bindings { get; set; } = Array.Empty<SerializableMemberBinding>();
}
