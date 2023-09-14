using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Serialized version of <see cref="MemberInfo"/>
/// </summary>
public class SerializedMemberInfo
{
    public string? Name { get; set; }
    public SerializedType? DeclaringType { get; set; }
}

public class MemberInfoSerializer
{
    protected TypeSerializer typeSerializer;

    public MemberInfoSerializer(TypeSerializer typeSerializer)
    {
        this.typeSerializer = typeSerializer;
    }

    public virtual SerializedMemberInfo Serialize(MemberInfo memberInfo)
    {
        if (memberInfo.DeclaringType == null)
        {
            throw new InvalidOperationException();
        }

        return new SerializedMemberInfo()
        {
            Name = memberInfo.Name,
            DeclaringType = typeSerializer.Serialize(memberInfo.DeclaringType)
        };
    }

    public virtual MemberInfo Deserialize(SerializedMemberInfo serializedMemberInfo)
    {
        if (serializedMemberInfo.Name == null)
        {
            throw new InvalidOperationException();
        }
        if (serializedMemberInfo.DeclaringType == null)
        {
            throw new InvalidOperationException();
        }

        var type = typeSerializer.Deserialize(serializedMemberInfo.DeclaringType);
        var members = type.GetMember(serializedMemberInfo.Name);

        if (members.IsEmpty())
        {
            throw new InvalidOperationException();
        }

        return members[0];
    }
}