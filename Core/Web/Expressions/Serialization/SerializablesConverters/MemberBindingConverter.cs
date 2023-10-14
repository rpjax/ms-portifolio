using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="MemberMemberBinding"/> and its serializable counterpart, <see cref="SerializableMemberBinding"/>.
/// </summary>
public interface IMemberBindingConverter : IBidirectionalConverter<MemberMemberBinding, SerializableMemberBinding>
{

}

/// <summary>
/// Provides an implementation for converting between <see cref="MemberMemberBinding"/> and <see cref="SerializableMemberBinding"/>.
/// </summary>
public class MemberBindingConverter : ConverterBase, IMemberBindingConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ConversionContext Context { get; }

    /// <summary>
    /// Gets the member info converter used for member info conversions.
    /// </summary>
    private IMemberInfoConverter MemberInfoConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberBindingConverter"/> class.
    /// </summary>
    /// <param name="parentContext">The parsing context.</param>
    public MemberBindingConverter(ConversionContext parentContext)
    {
        Context = parentContext.CreateChild("Member Binding Conversion");
        MemberInfoConverter = Context.GetDependency<IMemberInfoConverter>();
    }

    /// <summary>
    /// Converts a <see cref="MemberMemberBinding"/> to its serializable counterpart.
    /// </summary>
    /// <param name="instance">The member binding to convert.</param>
    /// <returns>The serializable representation of the member binding.</returns>
    public SerializableMemberBinding Convert(MemberMemberBinding instance)
    {
        return new()
        {
            MemberInfo = MemberInfoConverter.Convert(instance.Member),
            Bindings = instance.Bindings
                .Where(x => x is MemberMemberBinding)
                .Select(x => (MemberMemberBinding)x)
                .Transform(x => Convert(x)).ToArray()
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableMemberBinding"/> back to its original <see cref="MemberMemberBinding"/> form.
    /// </summary>
    /// <param name="sMemberBinding">The serializable member binding to convert.</param>
    /// <returns>The original member binding.</returns>
    public MemberMemberBinding Convert(SerializableMemberBinding sMemberBinding)
    {
        if (sMemberBinding.MemberInfo == null)
        {
            throw MissingArgumentException(nameof(sMemberBinding.MemberInfo));
        }
        if (sMemberBinding.Bindings == null)
        {
            throw MissingArgumentException(nameof(sMemberBinding.Bindings));
        }

        var memberInfo = MemberInfoConverter.Convert(sMemberBinding.MemberInfo);
        var bindings = sMemberBinding.Bindings
            .Transform(x => Convert(x))
            .ToArray();

        return Expression.MemberBind(memberInfo, bindings);
    }

}
