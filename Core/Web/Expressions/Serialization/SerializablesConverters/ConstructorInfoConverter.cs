using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="ConstructorInfo"/> and its serializable counterpart, <see cref="SerializableConstructorInfo"/>.
/// </summary>
public interface IConstructorInfoConverter : IBidirectionalConverter<ConstructorInfo, SerializableConstructorInfo>
{

}

/// <summary>
/// Provides an implementation for converting between <see cref="ConstructorInfo"/> and <see cref="SerializableConstructorInfo"/>.
/// </summary>
public class ConstructorInfoConverter : ConverterBase, IConstructorInfoConverter
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
    /// Initializes a new instance of the <see cref="ConstructorInfoConverter"/> class.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    public ConstructorInfoConverter(ConversionContext context)
    {
        Context = context;
        TypeConverter = Context.TypeConverter;
    }

    /// <summary>
    /// Converts a <see cref="ConstructorInfo"/> to its serializable counterpart.
    /// </summary>
    /// <param name="constructorInfo">The constructor information to convert.</param>
    /// <returns>The serializable representation of the constructor.</returns>
    public SerializableConstructorInfo Convert(ConstructorInfo constructorInfo)
    {
        return new()
        {
            DeclaringType = TypeConverter.Convert(constructorInfo.DeclaringType!),
            Parameters = constructorInfo
                .GetParameters()
                .Transform(x => TypeConverter.Convert(x.ParameterType))
                .ToArray()
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableConstructorInfo"/> back to its original <see cref="ConstructorInfo"/> form.
    /// </summary>
    /// <param name="sConstructorInfo">The serializable constructor information to convert.</param>
    /// <returns>The original constructor information.</returns>
    public ConstructorInfo Convert(SerializableConstructorInfo sConstructorInfo)
    {
        if (sConstructorInfo.DeclaringType == null)
        {
            throw MissingArgumentException(nameof(sConstructorInfo.DeclaringType));
        }

        var type = TypeConverter.Convert(sConstructorInfo.DeclaringType);
        var parameters = sConstructorInfo.Parameters
            .Transform(x => TypeConverter.Convert(x))
            .ToArray();

        var constructorInfo = type.GetConstructor(parameters);

        if (constructorInfo == null)
        {
            throw ConstructorNotFoundException(sConstructorInfo);
        }

        return constructorInfo;
    }

}
