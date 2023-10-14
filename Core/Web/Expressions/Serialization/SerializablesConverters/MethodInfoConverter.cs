using ModularSystem.Core;
using ModularSystem.Core.Reflection;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="MethodInfo"/> and its serializable counterpart, <see cref="SerializableMethodInfo"/>.
/// </summary>
public interface IMethodInfoConverter : IBidirectionalConverter<MethodInfo, SerializableMethodInfo>
{
}

/// <summary>
/// Provides an implementation for converting between <see cref="MethodInfo"/> and <see cref="SerializableMethodInfo"/>.
/// </summary>
public class MethodInfoConverter : ConverterBase, IMethodInfoConverter
{
    /// <summary>
    /// Gets the parsing context associated with the converter.
    /// </summary>
    protected override ConversionContext Context { get; }

    /// <summary>
    /// Gets the type converter used for type conversions.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInfoConverter"/> class.
    /// </summary>
    /// <param name="parentContext">The parsing context.</param>
    public MethodInfoConverter(ConversionContext parentContext)
    {
        Context = parentContext.CreateChild("Method Info Conversion");
        TypeConverter = Context.GetDependency<ITypeConverter>();
    }

    /// <summary>
    /// Converts a <see cref="MethodInfo"/> to its serializable counterpart.
    /// </summary>
    /// <param name="instance">The method information to convert.</param>
    /// <returns>The serializable representation of the method.</returns>
    public SerializableMethodInfo Convert(MethodInfo instance)
    {
        var genericArguments = instance.GetGenericArguments();
        var parameters = instance.GetParameters();

        return new()
        {
            BindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            IsGenericMethod = instance.IsGenericMethod,
            Name = instance.Name,
            DeclaringType = TypeConverter.Convert(instance.DeclaringType!),
            ReturnType = TypeConverter.Convert(instance.ReturnType!),
            GenericArguments = genericArguments
                .Transform(x => TypeConverter.Convert(x))
                .ToArray(),
            Parameters = parameters.Transform(x => TypeConverter.Convert(x.ParameterType)).ToArray()
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

        var name = sMethodInfo.Name;
        var declaringType = TypeConverter.Convert(sMethodInfo.DeclaringType!);
        var returnType = TypeConverter.Convert(sMethodInfo.ReturnType!);
        var parameters = sMethodInfo.Parameters
            .Transform(x => TypeConverter.Convert(x))
            .ToArray();

        var methodInfos = declaringType.GetManyMethodInfo(name, returnType, parameters).ToArray();

        if (methodInfos.IsEmpty())
        {
            throw MethodNotFoundException(sMethodInfo);
        }
        if (methodInfos.Length > 1)
        {
            throw AmbiguousMethodException(sMethodInfo);
        }

        var methodInfo = methodInfos.First();

        if (sMethodInfo.IsGenericMethod)
        {
            var genericArguments = sMethodInfo.GenericArguments
                .Transform(x => TypeConverter.Convert(x)).ToArray();

            methodInfo = methodInfo
                .MakeGenericMethod(genericArguments);
        }

        return methodInfo;
    }

}
