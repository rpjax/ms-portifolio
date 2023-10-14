using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a mechanism to convert between <see cref="MemberInfo"/> objects and their serializable representations.
/// </summary>
public interface IMemberInfoConverter : IBidirectionalConverter<MemberInfo, SerializableMemberInfo>
{

}

/// <summary>
/// A converter that facilitates the transformation between <see cref="MemberInfo"/> and its serializable form, <see cref="SerializableMemberInfo"/>.
/// </summary>
public class MemberInfoConverter : ConverterBase, IMemberInfoConverter
{
    /// <inheritdoc/>
    protected override ConversionContext Context { get; }

    /// <summary>
    /// Provides capabilities to convert types during the conversion process.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    /// <summary>
    /// Constructs a new instance of <see cref="MemberInfoConverter"/>, initialized with the given context and configuration.
    /// </summary>
    /// <param name="parentContext">The parsing context to be used during conversion.</param>
    public MemberInfoConverter(ConversionContext parentContext)
    {
        Context = parentContext.CreateChild("Member Info Conversion");
        TypeConverter = Context.GetDependency<ITypeConverter>();
    }

    /// <summary>
    /// Transforms a <see cref="MemberInfo"/> into its serializable counterpart.
    /// </summary>
    /// <param name="instance">The member information to be converted.</param>
    /// <returns>The serialized form of the provided member information.</returns>
    public SerializableMemberInfo Convert(MemberInfo instance)
    {
        return new()
        {
            DeclaringType = TypeConverter.Convert(instance.DeclaringType!),
            Name = instance.Name,
        };
    }

    /// <summary>
    /// Reverts a <see cref="SerializableMemberInfo"/> back to its original <see cref="MemberInfo"/> form.
    /// </summary>
    /// <param name="sMemberInfo">The serialized member information to be reverted.</param>
    /// <returns>The original member information derived from the serialized form.</returns>
    public MemberInfo Convert(SerializableMemberInfo sMemberInfo)
    {
        if (sMemberInfo.DeclaringType == null)
        {
            throw MissingArgumentException(nameof(sMemberInfo.DeclaringType));
        }
        if (sMemberInfo.Name == null)
        {
            throw MissingArgumentException(nameof(sMemberInfo.Name));
        }

        var type = TypeConverter.Convert(sMemberInfo.DeclaringType);
        var memberInfo = type.GetMember(sMemberInfo.Name);

        if (memberInfo.IsEmpty())
        {
            throw MemberNotFoundException(sMemberInfo);
        }
        if (memberInfo.Length > 1)
        {
            throw AmbiguousMemberException(sMemberInfo);
        }

        return memberInfo.First();
    }

}
