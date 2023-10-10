using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="MethodInfo"/> and its serializable counterpart, <see cref="SerializableMethodInfo"/>.
/// </summary>
public interface IMethodInfoConverter : IConverter<MethodInfo, SerializableMethodInfo>
{
}

/// <summary>
/// Provides an implementation for converting between <see cref="MethodInfo"/> and <see cref="SerializableMethodInfo"/>.
/// </summary>
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

    /// <summary>
    /// Gets the type converter used for type conversions.
    /// </summary>
    private ITypeConverter TypeConverter => Config.TypeConverter;

    /// <summary>
    /// Gets the parameter info converter used for parameter info conversions.
    /// </summary>
    private IParameterInfoConverter ParameterInfoConverter => Config.ParameterInfoConverter;

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInfoConverter"/> class.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="config">The configuration settings for the converter.</param>
    public MethodInfoConverter(ParsingContext context, Configs config)
    {
        Context = context;
        Config = config;
    }

    /// <summary>
    /// Converts a <see cref="MethodInfo"/> to its serializable counterpart.
    /// </summary>
    /// <param name="instance">The method information to convert.</param>
    /// <returns>The serializable representation of the method.</returns>
    public SerializableMethodInfo Convert(MethodInfo instance)
    {
        return new()
        {
            IsGenericMethod = instance.IsGenericMethod,
            Name = instance.Name,
            DeclaringType = TypeConverter.Convert(instance.DeclaringType!),
            ReturnType = TypeConverter.Convert(instance.ReturnType!),
            GenericArguments = instance
                .GetGenericArguments()
                .Transform(x => TypeConverter.Convert(x))
                .ToArray()
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableMethodInfo"/> back to its original <see cref="MethodInfo"/> form.
    /// </summary>
    /// <param name="sMethodInfo">The serializable method information to convert.</param>
    /// <returns>The original method information.</returns>
    public MethodInfo Convert(SerializableMethodInfo sMethodInfo)
    {
        if (sMethodInfo.Name == null)
        {
            throw MissingArgumentException(nameof(sMethodInfo.Name));
        }
        if (sMethodInfo.DeclaringType == null)
        {
            throw MissingArgumentException(nameof(sMethodInfo.DeclaringType));
        }
        if (sMethodInfo.ReturnType == null)
        {
            throw MissingArgumentException(nameof(sMethodInfo.ReturnType));
        }

        var declaringType = TypeConverter.Convert(sMethodInfo.DeclaringType!);
        var returnType = TypeConverter.Convert(sMethodInfo.ReturnType!);
        var methodInfo = declaringType
            .GetMethods(sMethodInfo.BindingFlags)
            .Where(x => sMethodInfo.Parameters
                .All(sP => x.GetParameters()
                    .Any(p => ParameterInfoConverter.Convert(sP) == p)))
            .ToArray();

        if (methodInfo.IsEmpty())
        {
            throw MethodNotFoundException(sMethodInfo);
        }
        if (methodInfo.Length > 1)
        {
            throw AmbiguousMethodException(sMethodInfo);
        }

        return methodInfo.First();
    }

    /// <summary>
    /// Represents configuration settings for the <see cref="MethodInfoConverter"/>.
    /// </summary>
    public class Configs
    {
        /// <summary>
        /// Gets or sets the type converter used for type conversions.
        /// </summary>
        public ITypeConverter TypeConverter { get; set; }

        /// <summary>
        /// Gets or sets the parameter info converter used for parameter info conversions.
        /// </summary>
        public IParameterInfoConverter ParameterInfoConverter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configs"/> class using the provided dependency container.
        /// </summary>
        /// <param name="dependencyContainer">The dependency container used to resolve required services.</param>
        public Configs(DependencyContainerObject dependencyContainer)
        {
            TypeConverter ??= dependencyContainer.GetInterface<ITypeConverter>();
            ParameterInfoConverter ??= dependencyContainer.GetInterface<IParameterInfoConverter>();
        }
    }
}
