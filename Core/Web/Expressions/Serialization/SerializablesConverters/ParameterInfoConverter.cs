using ModularSystem.Core;
using ModularSystem.Core.Reflection;
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
public class ParameterInfoConverter : Parser, IParameterInfoConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ParsingContext Context { get; }

    /// <summary>
    /// Gets the type converter used for type conversions.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInfoConverter"/> class.
    /// </summary>
    /// <param name="parentContext">The parsing context.</param>
    public ParameterInfoConverter(ParsingContext parentContext)
    {
        Context = parentContext.CreateChild("Parameter Info Conversion");
        TypeConverter = Context.GetDependency<ITypeConverter>();
    }

    /// <summary>
    /// Converts a <see cref="ParameterInfo"/> instance to its serializable representation.
    /// </summary>
    /// <param name="parameterInfo">The parameter information to convert.</param>
    /// <returns>The serializable representation of the parameter.</returns>
    public SerializableParameterInfo Convert(ParameterInfo parameterInfo)
    {
        return new SerializableParameterInfo()
        {
            MethodDeclaringType = TypeConverter.Convert(parameterInfo.Member.DeclaringType!),
            MethodName = parameterInfo.Member.Name,
            MethodParameters = parameterInfo.Member
                .TypeCast<MethodInfo>()
                .GetParameters()
                .Transform(x => TypeConverter.Convert(x.ParameterType))
                .ToArray(),
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
        if (sParameterInfo.MethodName == null)
        { 
            throw MissingArgumentException(nameof(sParameterInfo.MethodName));
        }
        if (sParameterInfo.MethodReturnType == null)
        {
            throw MissingArgumentException(nameof(sParameterInfo.MethodReturnType));
        }
        if (sParameterInfo.MethodParameters == null)
        {
            throw MissingArgumentException(nameof(sParameterInfo.MethodParameters));
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
        var methodName = sParameterInfo.MethodName;
        var methodReturnType = TypeConverter.Convert(sParameterInfo.MethodReturnType);
        var methodParamters = sParameterInfo.MethodParameters
            .Transform(st => TypeConverter.Convert(st))
            .ToArray();

        var methodInfo = methodDeclaringType
            .GetMethodInfo(methodName, methodReturnType, methodParamters);

        if(methodInfo == null)
        {
            throw MethodNotFoundException(methodDeclaringType, methodName);
        }

        var parameterType = TypeConverter.Convert(sParameterInfo.ParameterType);
        var paramters = methodInfo
            .GetParameters()
            .Where(p => p.Name == sParameterInfo.ParameterName)
            .Where(p => p.ParameterType == parameterType)
            .ToArray();

        if(paramters.IsEmpty())
        {
            throw ParameterNotFoundException(sParameterInfo);
        }
        if(paramters.Length > 1)
        {
            throw AmbiguousParameterException(sParameterInfo);
        }

        return paramters.First();
    }

}