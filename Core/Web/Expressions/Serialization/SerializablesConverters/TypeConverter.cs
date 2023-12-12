using ModularSystem.Core;
using System.Text;

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
    /// <param name="parentContext">The parsing context.</param>
    /// <param name="strategy">The strategy for this converter.</param>
    public TypeConverter(ConversionContext parentContext, TypeConversionStrategy strategy)
    {
        Context = parentContext.CreateChild("Type Conversion");
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
        return new SerializableType()
        {
            IsGenericMethodParameter = type.IsGenericMethodParameter,
            IsGenericParameter = type.IsGenericParameter,
            IsGenericType = type.IsGenericType,
            IsGenericTypeParameter = type.IsGenericTypeParameter,
            IsGenericTypeDefinition = type.IsGenericTypeDefinition,
            Name = type.Name,
            Namespace = type.Namespace,
            AssemblyQualifiedName =
                Strategy == TypeConversionStrategy.UseAssemblyName
                ? type.AssemblyQualifiedName
                : null,
            GenericTypeArguments = type.GenericTypeArguments.Transform(x => Convert(x)).ToArray(),
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

        Type? type = null;

        switch (Strategy)
        {
            case TypeConversionStrategy.UseAssemblyName:
                if (sType.AssemblyQualifiedName == null)
                {
                    throw MissingArgumentException(nameof(sType.AssemblyQualifiedName));
                }

                type = Type.GetType(sType.AssemblyQualifiedName);
                break;

            case TypeConversionStrategy.UseFullName:
                if (sType.FullName == null)
                {
                    throw MissingArgumentException(nameof(sType.FullName));
                }

                type = Type.GetType(sType.FullName);
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

    string GetQualifiedFullName(SerializableType sType)
    {
        if (sType.FullName == null)
        {
            throw MissingArgumentException(nameof(sType.FullName));
        }

        if (!sType.IsGenericTypeDefinition)
        {
            return sType.FullName;
        }

        var split = sType.FullName.Split('`');

        if (split.Length != 2)
        {
            throw InvalidTypeNameException(sType.FullName);
        }

        var fullname = split[0];
        var argCountStr = split[1];

        if (!int.TryParse(argCountStr, out var argCount))
        {
            throw InvalidTypeNameException(sType.FullName);
        }
        if (argCount != sType.GenericTypeArguments.Length)
        {
            throw InvalidTypeNameException(sType.FullName);
        }

        var argNames = sType.GenericTypeArguments.Select(x => x.FullName).ToArray();
        var strBuilder = new StringBuilder();

        strBuilder.Append($"{fullname}`{argCount}[");

        for (int i = 0; i < argNames.Length; i++)
        {
            var argName = argNames[i];
            var isLast = argNames.Length - 1 == i;

            if (argName == null)
            {
                throw InvalidTypeNameException(sType.FullName);
            }
            if (isLast)
            {
                strBuilder.Append($"[{argName}]");
            }
            else
            {
                strBuilder.Append($"[{argName}],");
            }
        }

        strBuilder.Append(']');
        return strBuilder.ToString();
    }
}
