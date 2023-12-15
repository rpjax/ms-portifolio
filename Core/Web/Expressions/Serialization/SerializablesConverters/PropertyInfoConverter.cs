using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="PropertyInfo"/> and its serializable counterpart, <see cref="SerializablePropertyInfo"/>.
/// </summary>
public interface IPropertyInfoConverter : IBidirectionalConverter<PropertyInfo, SerializablePropertyInfo>
{

}

/// <summary>
/// Provides an implementation for converting between <see cref="PropertyInfo"/> and <see cref="SerializablePropertyInfo"/>.
/// </summary>
public class PropertyInfoConverter : ConverterBase, IPropertyInfoConverter
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
    /// Initializes a new instance of the <see cref="PropertyInfoConverter"/> class.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    public PropertyInfoConverter(ConversionContext context)
    {
        Context = context;
        TypeConverter = context.TypeConverter;
    }

    /// <summary>
    /// Converts a <see cref="PropertyInfo"/> to its serializable counterpart.
    /// </summary>
    /// <param name="propertyInfo">The property information to convert.</param>
    /// <returns>The serializable representation of the <see cref="PropertyInfo"/>.</returns>
    public SerializablePropertyInfo Convert(PropertyInfo propertyInfo)
    {
        return new()
        {
            DeclaringType = TypeConverter.Convert(propertyInfo.DeclaringType!),
            Name = propertyInfo.Name,
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializablePropertyInfo"/> back to its original <see cref="PropertyInfo"/> form.
    /// </summary>
    /// <param name="sPropertyInfo">The serializable property information to convert.</param>
    /// <returns>The original property information.</returns>
    public PropertyInfo Convert(SerializablePropertyInfo sPropertyInfo)
    {
        if (sPropertyInfo.DeclaringType == null)
        {
            throw MissingArgumentException(nameof(sPropertyInfo.DeclaringType));
        }
        if (sPropertyInfo.Name == null)
        {
            throw MissingArgumentException(nameof(sPropertyInfo.Name));
        }

        var type = TypeConverter.Convert(sPropertyInfo.DeclaringType);
        var propertyInfo = type.GetProperty(sPropertyInfo.Name);

        if (propertyInfo == null)
        {
            throw PropertyNotFoundException(sPropertyInfo);
        }

        return propertyInfo;
    }

}
