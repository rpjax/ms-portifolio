using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a contract for converting between <see cref="ParameterInfo"/> and its serializable counterpart, <see cref="SerializableParameterInfo"/>. <br/>
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface IParameterInfoConverter
    : IBidirectionalConverter<ParameterInfo, SerializableParameterInfo, ConversionContext>
{
}

/// <summary>
/// Implements the conversion logic between <see cref="ParameterInfo"/> and <see cref="SerializableParameterInfo"/>. <br/>
/// Utilizes additional converters for type and method information.
/// </summary>
public class ParameterInfoConverter : ConverterBase, IParameterInfoConverter
{
    /// <summary>
    /// Gets the type converter used for type conversions.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    /// <summary>
    /// Gets the method info converter used for method information conversions.
    /// </summary>
    private IMethodInfoConverter MethodInfoConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ParameterInfoConverter"/> class.
    /// </summary>
    public ParameterInfoConverter(ITypeConverter typeConverter, IMethodInfoConverter methodInfoConverter)
    {
        TypeConverter = typeConverter;
        MethodInfoConverter = methodInfoConverter;
    }

    /// <summary>
    /// Converts a <see cref="ParameterInfo"/> instance to its serializable representation.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="parameterInfo">The parameter information to convert.</param>
    /// <returns>The serializable representation of the parameter information.</returns>
    public SerializableParameterInfo Convert(ConversionContext context, ParameterInfo parameterInfo)
    {
        var methodInfo = parameterInfo.Member.TypeCast<MethodInfo>();
        var methodParamters = methodInfo.GetParameters();

        return new SerializableParameterInfo()
        {
            MethodDeclaringType = TypeConverter.Convert(context, methodInfo.DeclaringType!),
            MethodInfo = MethodInfoConverter.Convert(context, methodInfo),
            ParameterName = parameterInfo.Name,
            ParameterType = TypeConverter.Convert(context, parameterInfo.ParameterType)
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableParameterInfo"/> back to its original <see cref="ParameterInfo"/> form.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="sParameterInfo">The serializable parameter information to convert.</param>
    /// <returns>The original <see cref="ParameterInfo"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required information for conversion is missing or ambiguous.</exception>
    public ParameterInfo Convert(ConversionContext context, SerializableParameterInfo sParameterInfo)
    {
        if (sParameterInfo.MethodDeclaringType == null)
        {
            throw MissingArgumentException(context, nameof(sParameterInfo.MethodDeclaringType));
        }
        if (sParameterInfo.MethodInfo == null)
        {
            throw MissingArgumentException(context, nameof(sParameterInfo.MethodInfo));
        }

        if (sParameterInfo.ParameterName == null)
        {
            throw MissingArgumentException(context, nameof(sParameterInfo.ParameterName));
        }
        if (sParameterInfo.ParameterType == null)
        {
            throw MissingArgumentException(context, nameof(sParameterInfo.ParameterType));
        }

        var methodDeclaringType = TypeConverter.Convert(context, sParameterInfo.MethodDeclaringType!);
        var methodInfo = MethodInfoConverter.Convert(context, sParameterInfo.MethodInfo);

        var parameterType = TypeConverter.Convert(context, sParameterInfo.ParameterType);
        var paramters = methodInfo
            .GetParameters()
            .Where(p => p.Name == sParameterInfo.ParameterName)
            .Where(p => p.ParameterType == parameterType)
            .ToArray();

        if (paramters.IsEmpty())
        {
            throw ParameterNotFoundException(context, sParameterInfo);
        }
        if (paramters.Length > 1)
        {
            throw AmbiguousParameterException(context, sParameterInfo);
        }

        return paramters.First();
    }

}