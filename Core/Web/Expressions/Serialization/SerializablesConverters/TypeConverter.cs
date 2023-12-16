using ModularSystem.Core;
using ModularSystem.Core.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Specifies the strategies available for converting types.
/// </summary>
public enum TypeConversionStrategy
{
    /// <summary>
    /// Uses the assembly-qualified name for type conversion, including the type name, 
    /// assembly name, version, culture, and public key token.
    /// </summary>
    UseAssemblyName,

    /// <summary>
    /// Uses the full name of the type for conversion, which includes the namespace and type name,
    /// but excludes the assembly information.
    /// </summary>
    UseFullName
}

/// <summary>
/// Defines a contract for converting between <see cref="Type"/> and <see cref="SerializableType"/>. <br/>
/// Utilizes a <see cref="ConversionContext"/> to maintain state and context during conversion.
/// </summary>
public interface ITypeConverter : IBidirectionalConverter<Type, SerializableType, ConversionContext>
{
}

/// <summary>
/// Provides functionality to convert between <see cref="Type"/> and <see cref="SerializableType"/>. <br/>
/// Handles different strategies for type conversion and manages generic types and anonymous types.
/// </summary>
public class TypeConverter : ConverterBase, ITypeConverter
{
    /// <summary>
    /// The strategy used for type conversion.
    /// </summary>
    private TypeConversionStrategy Strategy { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TypeConverter"/> class with the specified provider and strategy.
    /// </summary>
    /// <param name="strategy">The strategy to use for type conversion.</param>
    public TypeConverter(TypeConversionStrategy strategy)
    {
        Strategy = strategy;
    }

    /// <summary>
    /// Converts a <see cref="Type"/> to its serializable representation, <see cref="SerializableType"/>. <br/>
    /// Handles generic types, anonymous types, and respects the selected conversion strategy.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="type">The type to convert.</param>
    /// <returns>The serializable representation of the type.</returns>
    public SerializableType Convert(ConversionContext context, Type type)
    {
        var genericTypeDefinition = type.TryGetGenericTypeDefinition();

        if(genericTypeDefinition == type)
        {
            genericTypeDefinition = null;
        }

        var isAnonymous = type.IsAnonymous();

        var anonymousPropertiesDefinitions =
            isAnonymous
            ? type
                .GetProperties()
                .Transform(x => new SerializablePropertyDefinition(x.Name, Convert(context, x.PropertyType)))
                .ToArray()
            : Array.Empty<SerializablePropertyDefinition>();

        return new SerializableType()
        {
            IsGenericMethodParameter = type.IsGenericMethodParameter,
            IsGenericParameter = type.IsGenericParameter,
            IsGenericType = type.IsGenericType,
            IsGenericTypeParameter = type.IsGenericTypeParameter,
            IsGenericTypeDefinition = type.IsGenericTypeDefinition,
            IsAnonymousType = isAnonymous,
            Name = type.Name,
            Namespace = type.Namespace,
            AssemblyQualifiedName =
                Strategy == TypeConversionStrategy.UseAssemblyName
                ? type.GetQualifiedFullName()
                : null,
            GenericTypeArguments = type.GenericTypeArguments
                .Transform(x => Convert(context, x))
                .ToArray(),
            GenericTypeDefinition =
                genericTypeDefinition != null
                ? Convert(context, genericTypeDefinition)
                : null,
            AnonymousPropertyDefinitions = anonymousPropertiesDefinitions
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableType"/> back to its original <see cref="Type"/>. <br/>
    /// Handles deserialization considering the strategy used and manages generic and anonymous types.
    /// </summary>
    /// <param name="context">The conversion context.</param>
    /// <param name="sType">The serializable type to convert back to a <see cref="Type"/>.</param>
    /// <returns>The original <see cref="Type"/>.</returns>
    public Type Convert(ConversionContext context, SerializableType sType)
    {
        var isDeserializable = sType.FullNameIsAvailable() || sType.AssemblyNameIsAvailable();

        if (!isDeserializable)
        {
            throw new InvalidOperationException($"The serialized type does not have enough information to be deserialized. '{sType.GetFullName()}'.");
        }

        if (sType.IsAnonymousType)
        {
            return CreateAnonymousType(context, sType);
        }

        Type? type = null;

        switch (Strategy)
        {
            case TypeConversionStrategy.UseAssemblyName:

                type = CreateTypeUsingAssemblyName(context, sType);
                break;

            case TypeConversionStrategy.UseFullName:

                type = CreateTypeUsingFullName(context, sType);
                break;

            default:
                throw new InvalidOperationException("Invalid stategy value.");
        }

        if (type == null)
        {
            throw TypeNotFoundException(context, sType);
        }

        if (sType.IsGenericTypeDefinition)
        {
            type = type.MakeGenericType(sType.GenericTypeArguments.Transform(x => Convert(context, x)).ToArray());
        }

        return type;
    }

    private Type? CreateTypeUsingAssemblyName(ConversionContext context, SerializableType sType)
    {
        if (string.IsNullOrEmpty(sType.AssemblyQualifiedName))
        {
            throw MissingArgumentException(context, nameof(sType.AssemblyQualifiedName));
        }

        if (sType.IsGenericType)
        {
            var genericTypeDefinition = Type.GetType(sType.AssemblyQualifiedName);

            if(genericTypeDefinition == null)
            {
                throw TypeNotFoundException(context, sType.AssemblyQualifiedName);
            }

            var genericTypeArgs = sType.GenericTypeArguments
                .Transform(x => Convert(context, x))
                .ToArray();

            return genericTypeDefinition.MakeGenericType(genericTypeArgs);
        }

        return Type.GetType(sType.AssemblyQualifiedName);
    }

    private Type? CreateTypeUsingFullName(ConversionContext context, SerializableType sType)
    {
        if (string.IsNullOrEmpty(sType.FullName))
        {
            throw MissingArgumentException(context, nameof(sType.FullName));
        }

        if (sType.IsGenericType)
        {
            var genericTypeDefinition = Type.GetType(sType.FullName);

            if (genericTypeDefinition == null)
            {
                throw TypeNotFoundException(context, sType.FullName);
            }

            var genericTypeArgs = sType.GenericTypeArguments
                .Transform(x => Convert(context, x))
                .ToArray();

            return genericTypeDefinition.MakeGenericType(genericTypeArgs);
        }

        return Type.GetType(sType.FullName);
    }

    private Type CreateAnonymousType(ConversionContext context, SerializableType sType)
    {
        if (sType.AnonymousPropertyDefinitions.IsEmpty())
        {
            throw MissingArgumentException(context, nameof(sType.AnonymousPropertyDefinitions));
        }

        var properties = new List<AnonymousPropertyDefinition>(sType.AnonymousPropertyDefinitions.Length);
        var counter = 0;

        foreach (var item in sType.AnonymousPropertyDefinitions)
        {
            if(item.Name == null)
            {
                throw MissingArgumentException(context, $"{nameof(sType.AnonymousPropertyDefinitions)}[{counter}].{nameof(item.Name)}");
            }
            if(item.Type == null)
            {
                throw MissingArgumentException(context, $"{nameof(sType.AnonymousPropertyDefinitions)}[{counter}].{nameof(item.Type)}");
            }

            properties.Add(new AnonymousPropertyDefinition(item.Name, Convert(context, item.Type)));
            counter++;
        }

        var options = new AnonymousTypeCreationOptions()
        {
            CreateDefaultConstructor = true,
            CreateSetters = true,
            UseCache = true
        };
        var anonymousType = TypeCreator
            .CreateAnonymousType(properties, options);
        
        return anonymousType;
    }

}
