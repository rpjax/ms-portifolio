using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="ConstructorInfo"/> and its serializable counterpart, <see cref="SerializableConstructorInfo"/>. <br/>
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface IConstructorInfoConverter
    : IBidirectionalConverter<ConstructorInfo, SerializableConstructorInfo, ConversionContext>
{
}

/// <summary>
/// Provides an implementation for converting between <see cref="ConstructorInfo"/> and <see cref="SerializableConstructorInfo"/>. <br/>
/// Utilizes additional converters for type information.
/// </summary>
public class ConstructorInfoConverter : ConverterBase, IConstructorInfoConverter
{
    /// <summary>
    /// Gets the type converter used for type conversions.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConstructorInfoConverter"/> class.
    /// </summary>
    public ConstructorInfoConverter(ITypeConverter typeConverter)
    {
        TypeConverter = typeConverter;
    }

    /// <summary>
    /// Converts a <see cref="ConstructorInfo"/> to its serializable counterpart.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="constructorInfo">The constructor information to convert.</param>
    /// <returns>The serializable representation of the constructor.</returns>
    public SerializableConstructorInfo Convert(ConversionContext context, ConstructorInfo constructorInfo)
    {
        return new()
        {
            DeclaringType = TypeConverter.Convert(context, constructorInfo.DeclaringType!),
            Parameters = constructorInfo
                .GetParameters()
                .Transform(x => TypeConverter.Convert(context, x.ParameterType))
                .ToArray()
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableConstructorInfo"/> back to its original <see cref="ConstructorInfo"/> form.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="sConstructorInfo">The serializable constructor information to convert.</param>
    /// <returns>The original constructor information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required information for conversion is missing or cannot be found.</exception>
    public ConstructorInfo Convert(ConversionContext context, SerializableConstructorInfo sConstructorInfo)
    {
        if (sConstructorInfo.DeclaringType == null)
        {
            throw MissingArgumentException(context, nameof(sConstructorInfo.DeclaringType));
        }

        var type = TypeConverter.Convert(context, sConstructorInfo.DeclaringType);
        var parameters = sConstructorInfo.Parameters
            .Transform(x => TypeConverter.Convert(context, x))
            .ToArray();

        var constructorInfo = type.GetConstructor(parameters);

        if (constructorInfo == null)
        {
            throw ConstructorNotFoundException(context, sConstructorInfo);
        }

        return constructorInfo;
    }

}
