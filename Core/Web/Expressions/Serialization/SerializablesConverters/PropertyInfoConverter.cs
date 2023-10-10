using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="PropertyInfo"/> and its serializable counterpart, <see cref="SerializablePropertyInfo"/>.
/// </summary>
public interface IPropertyInfoConverter : IConverter<PropertyInfo, SerializablePropertyInfo>
{

}

/// <summary>
/// Provides an implementation for converting between <see cref="PropertyInfo"/> and <see cref="SerializablePropertyInfo"/>.
/// </summary>
public class PropertyInfoConverter : Converter, IPropertyInfoConverter
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
    /// Initializes a new instance of the <see cref="PropertyInfoConverter"/> class.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="configs">The configuration settings for the converter.</param>
    public PropertyInfoConverter(ParsingContext context, Configs configs)
    {
        Context = context;
        Config = configs;
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

        if(propertyInfo == null)
        {
            throw PropertyNotFoundException(sPropertyInfo);
        }

        return propertyInfo;
    }

    /// <summary>
    /// Represents configuration settings for the <see cref="PropertyInfoConverter"/>.
    /// </summary>
    public class Configs
    {
        /// <summary>
        /// Gets or sets the type converter used for type conversions.
        /// </summary>
        public ITypeConverter TypeConverter { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configs"/> class using the provided dependency container.
        /// </summary>
        /// <param name="dependencyContainer">The dependency container used to resolve required services.</param>
        public Configs(DependencyContainerObject dependencyContainer)
        {
            TypeConverter ??= dependencyContainer.GetInterface<ITypeConverter>();
        }
    }

}
