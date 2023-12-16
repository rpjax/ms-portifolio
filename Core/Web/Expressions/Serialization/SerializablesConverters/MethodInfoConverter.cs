using ModularSystem.Core;
using ModularSystem.Core.Reflection;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="MethodInfo"/> and its serializable counterpart, <see cref="SerializableMethodInfo"/>. <br/>
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface IMethodInfoConverter : IBidirectionalConverter<MethodInfo, SerializableMethodInfo, ConversionContext>
{
}

/// <summary>
/// Provides an implementation for converting between <see cref="MethodInfo"/> and <see cref="SerializableMethodInfo"/>. <br/>
/// Utilizes additional converters for type information.
/// </summary>
public class MethodInfoConverter : ConverterBase, IMethodInfoConverter
{
    /// <summary>
    /// Gets the type converter used for type conversions.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInfoConverter"/> class.
    /// </summary>
    public MethodInfoConverter(ITypeConverter typeConverter)
    {
        TypeConverter = typeConverter;
    }

    /// <summary>
    /// Converts a <see cref="MethodInfo"/> to its serializable counterpart.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="instance">The method information to convert.</param>
    /// <returns>The serializable representation of the method.</returns>
    public SerializableMethodInfo Convert(ConversionContext context, MethodInfo instance)
    {
        var genericArguments = instance.GetGenericArguments();
        var parameters = instance.GetParameters();

        return new()
        {
            BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            IsGenericMethod = instance.IsGenericMethod,
            Name = instance.Name,
            DeclaringType = TypeConverter.Convert(context, instance.DeclaringType!),
            ReturnType = TypeConverter.Convert(context, instance.ReturnType!),
            GenericArguments = genericArguments
                .Transform(x => TypeConverter.Convert(context, x))
                .ToArray(),
            Parameters = parameters.Transform(x => TypeConverter.Convert(context, x.ParameterType)).ToArray()
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableMethodInfo"/> back to its original <see cref="MethodInfo"/> form.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="sMethodInfo">The serializable method information to convert.</param>
    /// <returns>The original method information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required information for conversion is missing or ambiguous.</exception>
    public MethodInfo Convert(ConversionContext context, SerializableMethodInfo sMethodInfo)
    {
        if (sMethodInfo.Name == null)
        {
            throw MissingArgumentException(context, nameof(sMethodInfo.Name));
        }
        if (sMethodInfo.DeclaringType == null)
        {
            throw MissingArgumentException(context, nameof(sMethodInfo.DeclaringType));
        }
        if (sMethodInfo.ReturnType == null)
        {
            throw MissingArgumentException(context, nameof(sMethodInfo.ReturnType));
        }

        var name = sMethodInfo.Name;
        var declaringType = TypeConverter.Convert(context, sMethodInfo.DeclaringType!);
        var returnType = TypeConverter.Convert(context, sMethodInfo.ReturnType!);
        var parameters = sMethodInfo.Parameters
            .Transform(x => TypeConverter.Convert(context, x))
            .ToArray();

        var methodInfos = declaringType.GetManyMethodInfo(name, returnType, parameters).ToArray();

        if (methodInfos.IsEmpty())
        {
            throw MethodNotFoundException(context, sMethodInfo);
        }
        if (methodInfos.Length > 1)
        {
            throw AmbiguousMethodException(context, sMethodInfo);
        }

        var methodInfo = methodInfos.First();

        if (sMethodInfo.IsGenericMethod)
        {
            var genericArguments = sMethodInfo.GenericArguments
                .Transform(x => TypeConverter.Convert(context, x)).ToArray();

            methodInfo = methodInfo
                .MakeGenericMethod(genericArguments);
        }

        return methodInfo;
    }

}
