using ModularSystem.Core;
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
public class MethodInfoConverter : Parser, IMethodInfoConverter
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
    /// Gets the parameter info converter used for parameter info conversions.
    /// </summary>
    private IParameterInfoConverter ParameterInfoConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MethodInfoConverter"/> class.
    /// </summary>
    /// <param name="parentContext">The parsing context.</param>
    public MethodInfoConverter(ParsingContext parentContext)
    {
        Context = parentContext.CreateChild("Method Info Conversion");
        TypeConverter = Context.GetDependency<ITypeConverter>();
        ParameterInfoConverter = Context.GetDependency<IParameterInfoConverter>();
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

}
