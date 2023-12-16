using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a mechanism to convert between <see cref="MemberBinding"/> objects and their serializable representations. <br/>
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface IMemberBindingConverter
    : IBidirectionalConverter<MemberBinding, SerializableMemberBinding, ConversionContext>
{
}

/// <summary>
/// Implements the conversion logic between <see cref="MemberBinding"/> and <see cref="SerializableMemberBinding"/>. <br/>
/// This class supports conversion of different types of member bindings (assignment, member binding, list binding) <br/>
/// using provided converters for expressions and member information.
/// </summary>
public class MemberBindingConverter : ConverterBase, IMemberBindingConverter
{
    private IExpressionConverter ExpressionConverter { get; }
    private IMemberInfoConverter MemberInfoConverter { get; }
    private IElementInitConverter ElementInitConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MemberBindingConverter"/> class with dependencies for sub-conversions.
    /// </summary>
    /// <param name="expressionConverter">The converter used for expressions within member bindings.</param>
    /// <param name="memberInfoConverter">The converter used for member info within member bindings.</param>
    /// <param name="elementInitConverter">The converter used for element initializers within list bindings.</param>
    public MemberBindingConverter(
        IExpressionConverter expressionConverter, 
        IMemberInfoConverter memberInfoConverter,
        IElementInitConverter elementInitConverter
    )
    {
        ExpressionConverter = expressionConverter;
        MemberInfoConverter = memberInfoConverter;
        ElementInitConverter = elementInitConverter;
    }

    /// <summary>
    /// Converts a <see cref="MemberBinding"/> to its serializable counterpart.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="instance">The member binding to convert.</param>
    /// <returns>The serializable representation of the member binding.</returns>
    public SerializableMemberBinding Convert(ConversionContext context, MemberBinding instance)
    {
        if(instance is MemberAssignment assignment)
        {
            return ConvertMemberAssignment(context, assignment);
        }

        if(instance is MemberMemberBinding memberMemberBinding)
        {
            return ConvertMemberMemberBinding(context, memberMemberBinding);
        }

        if(instance is MemberListBinding memberListBinding)
        {
            return ConvertMemberListBinding(context, memberListBinding);
        }

        throw InvalidOperationException(context, "An invalid member binding type was encountered. This indicates a potential misconfiguration or an unsupported operation in the current conversion context.");
    }

    /// <summary>
    /// Converts a <see cref="SerializableMemberBinding"/> back to its original <see cref="MemberBinding"/> form.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="sMemberBinding">The serializable member binding to convert.</param>
    /// <returns>The original member binding.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required information for conversion is missing or cannot be found.</exception>
    public MemberBinding Convert(ConversionContext context, SerializableMemberBinding sMemberBinding)
    {
        var type = sMemberBinding.BindingType;

        if (type == MemberBindingType.Assignment)
        {
            return ConvertMemberAssignment(context, sMemberBinding);
        }

        if (type == MemberBindingType.MemberBinding)
        {
            return ConvertMemberMemberBinding(context, sMemberBinding);
        }

        if (type == MemberBindingType.ListBinding)
        {
            return ConvertMemberListBinding(context, sMemberBinding);
        }

        throw InvalidOperationException(context, "An invalid member binding type was encountered. This indicates a potential misconfiguration or an unsupported operation in the current conversion context.");
    }

    private SerializableMemberBinding ConvertMemberAssignment(ConversionContext context, MemberAssignment instance)
    {
        return new()
        {
            BindingType = instance.BindingType,
            MemberInfo = MemberInfoConverter.Convert(context, instance.Member),
            Expression = ExpressionConverter.Convert(context, instance.Expression)
        };
    }

    private SerializableMemberBinding ConvertMemberMemberBinding(ConversionContext context, MemberMemberBinding instance)
    {
        return new()
        {
            BindingType = instance.BindingType,
            MemberInfo = MemberInfoConverter.Convert(context, instance.Member),
            Bindings = instance.Bindings
                .Transform(x => Convert(context, x))
                .ToArray()
        };
    }

    private SerializableMemberBinding ConvertMemberListBinding(ConversionContext context, MemberListBinding instance)
    {
        return new()
        {
            BindingType = instance.BindingType,
            MemberInfo = MemberInfoConverter.Convert(context, instance.Member),
            Initializers = instance.Initializers
                .Transform(x => ElementInitConverter.Convert(context, x))
                .ToArray()
        };
    }
    
    //*
    // serializable converions
    //

    private MemberBinding ConvertMemberAssignment(ConversionContext context, SerializableMemberBinding sMemberBinding)
    {
        if (sMemberBinding.MemberInfo == null)
        {
            throw MissingArgumentException(context, nameof(sMemberBinding.MemberInfo));
        }
        if (sMemberBinding.Expression == null)
        {
            throw MissingArgumentException(context, nameof(sMemberBinding.Expression));
        }

        var memberInfo = MemberInfoConverter.Convert(context, sMemberBinding.MemberInfo);
        var expression = ExpressionConverter.Convert(context, sMemberBinding.Expression);

        return Expression.Bind(memberInfo, expression);
    }

    private MemberBinding ConvertMemberMemberBinding(ConversionContext context, SerializableMemberBinding sMemberBinding)
    {
        if (sMemberBinding.MemberInfo == null)
        {
            throw MissingArgumentException(context, nameof(sMemberBinding.MemberInfo));
        }
        if (sMemberBinding.Bindings == null)
        {
            throw MissingArgumentException(context, nameof(sMemberBinding.Bindings));
        }

        var memberInfo = MemberInfoConverter.Convert(context, sMemberBinding.MemberInfo);
        var bindings = sMemberBinding.Bindings
            .Transform(x => Convert(context, x))
            .ToArray();

        return Expression.MemberBind(memberInfo, bindings);
    }

    private MemberBinding ConvertMemberListBinding(ConversionContext context, SerializableMemberBinding sMemberBinding)
    {
        if (sMemberBinding.MemberInfo == null)
        {
            throw MissingArgumentException(context, nameof(sMemberBinding.MemberInfo));
        }

        var memberInfo = MemberInfoConverter.Convert(context, sMemberBinding.MemberInfo);
        var initializers = sMemberBinding.Initializers
            .Transform(x => ElementInitConverter.Convert(context, x))
            .ToArray();

        return Expression.ListBind(memberInfo, initializers);
    }

}
