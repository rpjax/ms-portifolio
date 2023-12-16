using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a mechanism to convert between <see cref="MemberInfo"/> objects and their serializable representations. <br/>
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface IMemberInfoConverter : IBidirectionalConverter<MemberInfo, SerializableMemberInfo, ConversionContext>
{

}

/// <summary>
/// A converter that facilitates the transformation between <see cref="MemberInfo"/> and its serializable form, <see cref="SerializableMemberInfo"/>. <br/>
/// Utilizes additional converters for type information.
/// </summary>
public class MemberInfoConverter : ConverterBase, IMemberInfoConverter
{
    /// <summary>
    /// Provides capabilities to convert types during the conversion process.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    /// <summary>
    /// Constructs a new instance of <see cref="MemberInfoConverter"/>, initialized with a dependency provider.
    /// </summary>
    public MemberInfoConverter(ITypeConverter typeConverter)
    {
        TypeConverter = typeConverter;
    }

    /// <summary>
    /// Transforms a <see cref="MemberInfo"/> into its serializable counterpart.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="instance">The member information to be converted.</param>
    /// <returns>The serialized form of the provided member information.</returns>
    public SerializableMemberInfo Convert(ConversionContext context, MemberInfo instance)
    {
        return new()
        {
            DeclaringType = TypeConverter.Convert(context, instance.DeclaringType!),
            Name = instance.Name,
        };
    }

    /// <summary>
    /// Reverts a <see cref="SerializableMemberInfo"/> back to its original <see cref="MemberInfo"/> form.
    /// </summary> <br/>
    /// <param name="context">The conversion context.</param>
    /// <param name="sMemberInfo">The serialized member information to be reverted.</param>
    /// <returns>The original member information derived from the serialized form.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required information for conversion is missing or ambiguous.</exception>
    public MemberInfo Convert(ConversionContext context, SerializableMemberInfo sMemberInfo)
    {
        if (sMemberInfo.DeclaringType == null)
        {
            throw MissingArgumentException(context, nameof(sMemberInfo.DeclaringType));
        }
        if (sMemberInfo.Name == null)
        {
            throw MissingArgumentException(context, nameof(sMemberInfo.Name));
        }

        var type = TypeConverter.Convert(context, sMemberInfo.DeclaringType);
        var memberInfo = type.GetMember(sMemberInfo.Name);

        if (memberInfo.IsEmpty())
        {
            throw MemberNotFoundException(context, sMemberInfo);
        }
        if (memberInfo.Length > 1)
        {
            throw AmbiguousMemberException(context, sMemberInfo);
        }

        return memberInfo.First();
    }

}
