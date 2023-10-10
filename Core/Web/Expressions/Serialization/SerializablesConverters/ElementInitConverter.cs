using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="ElementInit"/> and its serializable counterpart, <see cref="SerializableElementInit"/>.
/// </summary>
public interface IElementInitConverter : IConverter<ElementInit, SerializableElementInit>
{

}

/// <summary>
/// Provides an implementation for converting between <see cref="ElementInit"/> and <see cref="SerializableElementInit"/>.
/// </summary>
public class ElementInitConverter : Converter, IElementInitConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ParsingContext Context { get; }

    /// <summary>
    /// Gets the configuration settings for the converter.
    /// </summary>
    private Configs Config { get; }

    /// <summary>
    /// Gets the method info converter used for method info conversions.
    /// </summary>
    private IMethodInfoConverter MethodInfoConverter => Config.MethodInfoConverter;

    /// <summary>
    /// Gets the expression converter used for expression conversions.
    /// </summary>
    private IExpressionConverter ExpressionConverter => Config.ExpressionConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ElementInitConverter"/> class.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="config">The configuration settings for the converter.</param>
    public ElementInitConverter(ParsingContext context, Configs config)
    {
        Context = context;
        Config = config;
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

    /// <summary>
    /// Represents configuration settings for the <see cref="ElementInitConverter"/>.
    /// </summary>
    public class Configs
    {
        /// <summary>
        /// Gets or sets the method info converter used for method info conversions.
        /// </summary>
        public IMethodInfoConverter MethodInfoConverter { get; set; }

        /// <summary>
        /// Gets or sets the expression converter used for expression conversions.
        /// </summary>
        public IExpressionConverter ExpressionConverter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configs"/> class using the provided dependency container.
        /// </summary>
        /// <param name="dependencyContainer">The dependency container used to resolve required services.</param>
        public Configs(DependencyContainerObject dependencyContainer)
        {
            MethodInfoConverter ??= dependencyContainer.GetInterface<IMethodInfoConverter>();
            ExpressionConverter ??= dependencyContainer.GetInterface<IExpressionConverter>();
        }
    }
}
