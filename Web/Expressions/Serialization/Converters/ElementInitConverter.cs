using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="ElementInit"/> and its serializable counterpart, <see cref="SerializableElementInit"/>.
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface IElementInitConverter
    : IBidirectionalConverter<ElementInit, SerializableElementInit, ConversionContext>
{
}

/// <summary>
/// Provides an implementation for converting between <see cref="ElementInit"/> and <see cref="SerializableElementInit"/>.
/// Utilizes additional converters for method information and expression conversions.
/// </summary>
public class ElementInitConverter : ConverterBase, IElementInitConverter
{
    /// <summary>
    /// Gets the method info converter used for method info conversions.
    /// </summary>
    private IMethodInfoConverter MethodInfoConverter { get; }

    /// <summary>
    /// Gets the expression converter used for expression conversions.
    /// </summary>
    private IExpressionConverter ExpressionConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementInitConverter"/> class.
    /// </summary>
    public ElementInitConverter(IMethodInfoConverter methodInfoConverter, IExpressionConverter expressionConverter)
    {
        MethodInfoConverter = methodInfoConverter;
        ExpressionConverter = expressionConverter;
    }

    /// <summary>
    /// Converts an <see cref="ElementInit"/> to its serializable counterpart.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="instance">The element initialization to convert.</param>
    /// <returns>The serializable representation of the element initialization.</returns>
    public SerializableElementInit Convert(ConversionContext context, ElementInit instance)
    {
        return new()
        {
            MethodInfo = MethodInfoConverter.Convert(context, instance.AddMethod),
            Arguments = instance.Arguments
                .Transform(x => ExpressionConverter.Convert(context, x))
                .ToArray()
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableElementInit"/> back to its original <see cref="ElementInit"/> form.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="sElementInit">The serializable element initialization to convert.</param>
    /// <returns>The original element initialization.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required information for conversion is missing or cannot be found.</exception>
    public ElementInit Convert(ConversionContext context, SerializableElementInit sElementInit)
    {
        if (sElementInit.MethodInfo == null)
        {
            throw MissingArgumentException(context, nameof(sElementInit.MethodInfo));
        }
        if (sElementInit.Arguments == null)
        {
            throw MissingArgumentException(context, nameof(sElementInit.Arguments));
        }

        var methodInfo = MethodInfoConverter.Convert(context, sElementInit.MethodInfo);
        var arguments = sElementInit.Arguments
            .Transform(x => ExpressionConverter.Convert(context, x))
            .ToArray();

        return Expression.ElementInit(methodInfo, arguments);
    }

}
