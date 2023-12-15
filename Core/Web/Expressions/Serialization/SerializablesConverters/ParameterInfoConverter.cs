using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a contract for converting between <see cref="ParameterInfo"/> and its serializable counterpart, <see cref="SerializableParameterInfo"/>.
/// </summary>
public interface IParameterInfoConverter : IBidirectionalConverter<ParameterInfo, SerializableParameterInfo>
{
}

/// <summary>
/// Implements the conversion logic between <see cref="ParameterInfo"/> and <see cref="SerializableParameterInfo"/>.
/// </summary>
public class ParameterInfoConverter : ConverterBase, IParameterInfoConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ConversionContext Context { get; }

    /// <summary>
    /// Gets the type converter used for type conversions.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    private IMethodInfoConverter MethodInfoConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInfoConverter"/> class.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    public ParameterInfoConverter(ConversionContext context)
    {
        Context = context;
        TypeConverter = context.TypeConverter;
        MethodInfoConverter = context.MethodInfoConverter;
    }

    /// <summary>
    /// Converts a <see cref="ParameterInfo"/> instance to its serializable representation.
    /// </summary>
    /// <param name="parameterInfo">The parameter information to convert.</param>
    /// <returns>The serializable representation of the parameter.</returns>
    public SerializableParameterInfo Convert(ParameterInfo parameterInfo)
    {
        var methodInfo = parameterInfo.Member.TypeCast<MethodInfo>();
        var methodParamters = methodInfo.GetParameters();

        return new SerializableParameterInfo()
        {
            MethodDeclaringType = TypeConverter.Convert(methodInfo.DeclaringType!),
            MethodInfo = MethodInfoConverter.Convert(methodInfo),
            ParameterName = parameterInfo.Name,
            ParameterType = TypeConverter.Convert(parameterInfo.ParameterType)
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableParameterInfo"/> back to its original <see cref="ParameterInfo"/> form.
    /// </summary>
    /// <param name="sParameterInfo">The serializable parameter information to convert.</param>
    /// <returns>The original parameter information.</returns>
    public ParameterInfo Convert(SerializableParameterInfo sParameterInfo)
    {
        if (sParameterInfo.MethodDeclaringType == null)
        {
            throw MissingArgumentException(nameof(sParameterInfo.MethodDeclaringType));
        }
        if (sParameterInfo.MethodInfo == null)
        {
            throw MissingArgumentException(nameof(sParameterInfo.MethodInfo));
        }

        if (sParameterInfo.ParameterName == null)
        {
            throw MissingArgumentException(nameof(sParameterInfo.ParameterName));
        }
        if (sParameterInfo.ParameterType == null)
        {
            throw MissingArgumentException(nameof(sParameterInfo.ParameterType));
        }

        var methodDeclaringType = TypeConverter.Convert(sParameterInfo.MethodDeclaringType!);
        var methodInfo = MethodInfoConverter.Convert(sParameterInfo.MethodInfo);

        var parameterType = TypeConverter.Convert(sParameterInfo.ParameterType);
        var paramters = methodInfo
            .GetParameters()
            .Where(p => p.Name == sParameterInfo.ParameterName)
            .Where(p => p.ParameterType == parameterType)
            .ToArray();

        if (paramters.IsEmpty())
        {
            throw ParameterNotFoundException(sParameterInfo);
        }
        if (paramters.Length > 1)
        {
            throw AmbiguousParameterException(sParameterInfo);
        }

        return paramters.First();
    }

}