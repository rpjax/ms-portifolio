using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

/// <inheritdoc/>
public interface ITypeConverter : IConverter<Type, SerializableType>
{

}

/// <inheritdoc/>
public class TypeConverter : ITypeConverter
{
    private Configs Config { get; }

    public TypeConverter(Configs config)
    {
        Config = config;
    }

    /// <inheritdoc/>
    public SerializableType Convert(Type type)
    {
        return new SerializableType()
        {
            IsGenericTypeDefinition = type.IsGenericTypeDefinition,
            Name = type.Name,
            Namespace = type.Namespace,
            AssemblyQualifiedName = type.AssemblyQualifiedName,
            GenericTypeArguments = type.GenericTypeArguments.Transform(x => Convert(x)).ToArray(),
        };
    }

    /// <inheritdoc/>
    public Type Convert(SerializableType serializableType)
    {
        AppDomain domain = AppDomain.CurrentDomain;
        Assembly[] assemblies = domain.GetAssemblies();
        List<Type> types = new List<Type>();
        Type? deserializedType = null;

        foreach (Assembly assembly in assemblies)
        {
            types.AddRange(assembly.GetTypes());
        }

        var isDeserializable = serializableType.FullNameIsAvailable() || serializableType.AssemblyNameIsAvailable();

        if (!isDeserializable)
        {
            throw new InvalidOperationException($"The serialized type does not have enough information to be deserialized. '{serializableType.GetFullName()}'.");
        }

        if (deserializedType == null && serializableType.AssemblyNameIsAvailable() && Config.UseAssemblyName)
        {
            deserializedType = Type.GetType(serializableType.AssemblyQualifiedName!);
        }

        if (deserializedType == null && serializableType.FullNameIsAvailable() && Config.UseFullName)
        {
            deserializedType = Type.GetType(serializableType.GetFullName());
        }

        if (deserializedType == null)
        {
            throw new InvalidOperationException($"Could not find the serialized type in the current assembly '{serializableType.GetFullName()}'.");
        }

        if (serializableType.IsGenericTypeDefinition && serializableType.ContainsGenericArguments())
        {
            deserializedType = deserializedType.MakeGenericType(serializableType.GenericTypeArguments.Transform(x => Convert(x)).ToArray());
        }

        return deserializedType;
    }

    /// <summary>
    /// Represents configuration settings for the <see cref="ElementInitConverter"/>.
    /// </summary>
    public class Configs
    {
        public bool UseAssemblyName { get; set; }
        public bool UseFullName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Configs"/> class using the provided dependency container.
        /// </summary>
        /// <param name="dependencyContainer">The dependency container used to resolve required services.</param>
        public Configs(DependencyContainerObject dependencyContainer)
        {

        }
    }
}
