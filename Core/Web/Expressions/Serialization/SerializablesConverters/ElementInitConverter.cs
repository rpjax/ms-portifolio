using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="ElementInit"/> and its serializable counterpart, <see cref="SerializableElementInit"/>.
/// </summary>
public interface IElementInitConverter : IBidirectionalConverter<ElementInit, SerializableElementInit>
{

}

/// <summary>
/// Provides an implementation for converting between <see cref="ElementInit"/> and <see cref="SerializableElementInit"/>.
/// </summary>
public class ElementInitConverter : Parser, IElementInitConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ParsingContext Context { get; }

    /// <summary>
    /// Gets the method info converter used for method info conversions.
    /// </summary>
    private IMethodInfoConverter MethodInfoConverter { get; }

    /// <summary>
    /// Gets the expression converter used for expression conversions.
    /// </summary>
    private IExpressionConverter ExpressionConverter => Context.GetDependency<IExpressionConverter>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementInitConverter"/> class.
    /// </summary>
    /// <param name="parentContext">The parsing context.</param>
    public ElementInitConverter(ParsingContext parentContext)
    {
        Context = parentContext.CreateChild("Element Init Conversion");
        MethodInfoConverter = Context.GetDependency<IMethodInfoConverter>();
    }

    /// <summary>
    /// Converts an <see cref="ElementInit"/> to its serializable counterpart.
    /// </summary>
    /// <param name="instance">The element initialization to convert.</param>
    /// <returns>The serializable representation of the element initialization.</returns>
    public SerializableElementInit Convert(ElementInit instance)
    {
        return new()
        {
            MethodInfo = MethodInfoConverter.Convert(instance.AddMethod),
            Arguments = instance.Arguments
                .Transform(x => ExpressionConverter.Convert(x))
                .ToArray()
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableElementInit"/> back to its original <see cref="ElementInit"/> form.
    /// </summary>
    /// <param name="sElementInit">The serializable element initialization to convert.</param>
    /// <returns>The original element initialization.</returns>
    public ElementInit Convert(SerializableElementInit sElementInit)
    {
        if (sElementInit.MethodInfo == null)
        {
            throw MissingArgumentException(nameof(sElementInit.MethodInfo));
        }
        if (sElementInit.Arguments == null)
        {
            throw MissingArgumentException(nameof(sElementInit.Arguments));
        }

        var methodInfo = MethodInfoConverter.Convert(sElementInit.MethodInfo);
        var arguments = sElementInit.Arguments
            .Transform(x => ExpressionConverter.Convert(x))
            .ToArray();

        return Expression.ElementInit(methodInfo, arguments);
    }

}
