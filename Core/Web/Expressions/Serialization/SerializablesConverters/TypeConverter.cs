using ModularSystem.Core;
using ModularSystem.Core.Reflection;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Defines a contract for converting between <see cref="Type"/> and <see cref="SerializableType"/>.
/// </summary>
public interface ITypeConverter : IBidirectionalConverter<Type, SerializableType>
{
}

///// <summary>
///// Responsible for initializing the type converter.
///// </summary>
//internal class TypeConverterInitializer : Initializer
//{
//    /// <summary>
//    /// Performs internal initialization tasks for the type converter.
//    /// </summary>
//    /// <param name="options">Initialization options.</param>
//    /// <returns>A task representing the asynchronous operation.</returns>
//    public override Task InternalInitAsync(Options options)
//    {
//        //_ = TypeConverter.InitCache();
//        return base.InternalInitAsync(options);
//    }
//}

/// <summary>
/// Provides functionality to convert between <see cref="Type"/> and <see cref="SerializableType"/>.
/// </summary>
public class TypeConverter : ConverterBase, ITypeConverter
{
    /// <summary>
    /// Specifies the strategies available for converting types.
    /// </summary>
    public enum TypeConversionStrategy
    {
        /// <summary>
        /// Indicates that the type conversion should utilize the assembly-qualified name.
        /// This includes the type name, assembly name, version, culture, and public key token.
        /// </summary>
        UseAssemblyName,

        /// <summary>
        /// Indicates that the type conversion should utilize the full name of the type.
        /// This includes the namespace and the type name, but not the assembly information.
        /// </summary>
        UseFullName
    }

    /// <summary>
    /// Gets the parsing context associated with this converter.
    /// </summary>
    protected override ConversionContext Context { get; }

    /// <summary>
    /// The strategy used for type conversion.
    /// </summary>
    private TypeConversionStrategy Strategy { get; }

    /// <summary>
    /// Constructs a new instance of the <see cref="TypeConverter"/>.
    /// </summary>
    /// <param name="context">The parsing context.</param>
    /// <param name="strategy">The strategy for this converter.</param>
    public TypeConverter(ConversionContext context, TypeConversionStrategy strategy)
    {
        Context = context;
        Strategy = strategy;
    }

    ///// <summary>
    ///// Initializes the cache of assemblies.
    ///// </summary>
    ///// <returns>A queryable collection of assemblies.</returns>
    //public static IQueryable<Assembly> InitCache()
    //{
    //    return Assemblies;
    //}

    /// <summary>
    /// Converts a <see cref="Type"/> to its serializable representation.
    /// </summary>
    /// <param name="type">The type to convert.</param>
    /// <returns>The serializable representation of the type.</returns>
    public SerializableType Convert(Type type)
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
                .Transform(x => new SerializablePropertyDefinition(x.Name, Convert(x.PropertyType)))
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
                .Transform(x => Convert(x))
                .ToArray(),
            GenericTypeDefinition =
                genericTypeDefinition != null
                ? Convert(genericTypeDefinition)
                : null,
            AnonymousPropertyDefinitions = anonymousPropertiesDefinitions
        };
    }

    /// <summary>
    /// Converts a <see cref="SerializableType"/> back to its original <see cref="Type"/>.
    /// </summary>
    /// <param name="sType">The serializable type to convert.</param>
    /// <returns>The original type.</returns>
    public Type Convert(SerializableType sType)
    {
        var isDeserializable = sType.FullNameIsAvailable() || sType.AssemblyNameIsAvailable();

        if (!isDeserializable)
        {
            throw new InvalidOperationException($"The serialized type does not have enough information to be deserialized. '{sType.GetFullName()}'.");
        }

        if (sType.IsAnonymousType)
        {
            return CreateAnonymousType(sType);
        }

        Type? type = null;

        switch (Strategy)
        {
            case TypeConversionStrategy.UseAssemblyName:

                type = CreateTypeUsingAssemblyName(sType);
                break;

            case TypeConversionStrategy.UseFullName:

                type = CreateTypeUsingFullName(sType);
                break;

            default:
                throw new InvalidOperationException("Invalid stategy value.");
        }

        if (type == null)
        {
            throw TypeNotFoundException(sType);
        }

        if (sType.IsGenericTypeDefinition)
        {
            type = type.MakeGenericType(sType.GenericTypeArguments.Transform(x => Convert(x)).ToArray());
        }

        return type;
    }

    private Type? CreateTypeUsingAssemblyName(SerializableType sType)
    {
        if (string.IsNullOrEmpty(sType.AssemblyQualifiedName))
        {
            throw MissingArgumentException(nameof(sType.AssemblyQualifiedName));
        }

        if (sType.IsGenericType)
        {
            var genericTypeDefinition = Type.GetType(sType.AssemblyQualifiedName);

            if(genericTypeDefinition == null)
            {
                throw TypeNotFoundException(sType.AssemblyQualifiedName);
            }

            var genericTypeArgs = sType.GenericTypeArguments
                .Transform(x => Convert(x))
                .ToArray();

            return genericTypeDefinition.MakeGenericType(genericTypeArgs);
        }

        return Type.GetType(sType.AssemblyQualifiedName);
    }

    private Type? CreateTypeUsingFullName(SerializableType sType)
    {
        if (string.IsNullOrEmpty(sType.FullName))
        {
            throw MissingArgumentException(nameof(sType.FullName));
        }

        if (sType.IsGenericType)
        {
            var genericTypeDefinition = Type.GetType(sType.FullName);

            if (genericTypeDefinition == null)
            {
                throw TypeNotFoundException(sType.FullName);
            }

            var genericTypeArgs = sType.GenericTypeArguments
                .Transform(x => Convert(x))
                .ToArray();

            return genericTypeDefinition.MakeGenericType(genericTypeArgs);
        }

        return Type.GetType(sType.FullName);
    }

    private Type CreateAnonymousType(SerializableType sType)
    {
        if (sType.AnonymousPropertyDefinitions.IsEmpty())
        {
            throw new Exception();
        }

        var properties = new List<AnonymousPropertyDefinition>(sType.AnonymousPropertyDefinitions.Length);

        foreach (var item in sType.AnonymousPropertyDefinitions)
        {
            if(item.Name == null)
            {
                throw new Exception();
            }
            if(item.Type == null)
            {
                throw new Exception();
            }

            properties.Add(new AnonymousPropertyDefinition(item.Name, Convert(item.Type)));
        }

        var options = new AnonymousTypeCreationOptions()
        {
            CreateDefaultConstructor = true,
            CreateSetters = true
        };
        var anonymousType = TypeCreator
            .CreateAnonymousType(properties, options);
        
        return anonymousType;
    }

}
