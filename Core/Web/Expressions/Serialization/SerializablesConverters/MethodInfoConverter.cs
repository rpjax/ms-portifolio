using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <inheritdoc/>
public interface IMethodInfoConverter : IConverter<MethodInfo, SerializableMethodInfo>
{

}

/// <inheritdoc/>
public class MethodInfoConverter : Converter, IMethodInfoConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ParsingContext Context { get; }

    /// <summary>
    /// Gets the configuration settings for the converter.
    /// </summary>
    private Configs Config { get; }

    public MethodInfoConverter(ParsingContext context, Configs config)
    {
        Context = context;
        Config = config;
    }

    /// <inheritdoc/>
    public SerializableMethodInfo Convert(MethodInfo instance)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    public MethodInfo Convert(SerializableMethodInfo instance)
    {
        throw new NotImplementedException();
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
