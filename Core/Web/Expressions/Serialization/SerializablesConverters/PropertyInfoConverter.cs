using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="PropertyInfo"/> and its serializable counterpart, <see cref="SerializablePropertyInfo"/>. <br/>
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface IPropertyInfoConverter : IBidirectionalConverter<PropertyInfo, SerializablePropertyInfo, ConversionContext>
{
}

/// <summary>
/// Provides an implementation for converting between <see cref="PropertyInfo"/> and <see cref="SerializablePropertyInfo"/>. <br/>
/// Utilizes additional converters for type information.
/// </summary>
public class PropertyInfoConverter : ConverterBase, IPropertyInfoConverter
{
    /// <summary>
    /// Gets the type converter used for type conversions.
    /// </summary>
    private ITypeConverter TypeConverter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyInfoConverter"/> class.
    /// </summary>
    public PropertyInfoConverter(ITypeConverter typeConverter)
    {
        TypeConverter = typeConverter;
    }

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> to its serializable counterpart.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="propertyInfo">The property information to convert.</param>
    /// <returns>The serializable representation of the <see cref="PropertyInfo"/>.</returns>
    public SerializablePropertyInfo Convert(ConversionContext context, PropertyInfo propertyInfo)
    {
        return new SerializablePropertyInfo
        {
            DeclaringType = TypeConverter.Convert(context, propertyInfo.DeclaringType!),
            Name = propertyInfo.Name,
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializablePropertyInfo"/> back to its original <see cref="PropertyInfo"/> form.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="sPropertyInfo">The serializable property information to convert.</param>
    /// <returns>The original property information.</returns>
    /// <exception cref="InvalidOperationException">Thrown when required information for conversion is missing or cannot be found.</exception>
    public PropertyInfo Convert(ConversionContext context, SerializablePropertyInfo sPropertyInfo)
    {
        if (sPropertyInfo.DeclaringType == null)
        {
            throw MissingArgumentException(context, nameof(sPropertyInfo.DeclaringType));
        }
        if (sPropertyInfo.Name == null)
        {
            throw MissingArgumentException(context, nameof(sPropertyInfo.Name));
        }

        var type = TypeConverter.Convert(context, sPropertyInfo.DeclaringType);
        var propertyInfo = type.GetProperty(sPropertyInfo.Name);

        if (propertyInfo == null)
        {
            throw PropertyNotFoundException(context, sPropertyInfo);
        }

        return propertyInfo;
    }

}
