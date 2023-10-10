using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

public class TypeMapper : IMapper<SerializableType, Type> // pre serialization mapping
{
    Dictionary<string, Func<SerializableType, Type>> map = new();
    protected TypeSerializer? typeSerializer;

    public TypeMapper(TypeSerializer? serializer = null)
    {
        typeSerializer = serializer;

        InternalInit();
        Init();
    }

    public Type? Get(SerializableType serializedType)
    {
        var lambda = map.Get(serializedType.GetFullName());

        if (lambda != null)
        {
            return lambda(serializedType);
        }

        return null;
    }

    public void Map(SerializableType source, Type target)
    {
        if (!source.FullNameIsAvailable())
        {
            throw new InvalidOperationException();
        }

        map[source.GetFullName()] = x => target;
    }

    public void Map(SerializableType source, Func<SerializableType, Type> lambda)
    {
        if (!source.FullNameIsAvailable())
        {
            throw new InvalidOperationException();
        }

        map[source.GetFullName()] = lambda;
    }

    public void Map(Type source, Type target)
    {
        if (source.FullName == null)
        {
            throw new InvalidOperationException();
        }

        map[source.FullName] = x => target;
    }

    public void Map(Type source, Func<SerializableType, Type> lambda)
    {
        if (source.FullName == null)
        {
            throw new InvalidOperationException();
        }

        map[source.FullName] = lambda;
    }

    public void SetTypeSerializer(TypeSerializer serializer)
    {
        typeSerializer = serializer;
    }

    void InternalInit()
    {
        Map(typeof(Func<,>), x =>
        {
            var arguments = x.GenericTypeArguments.Transform(x => typeSerializer.Deserialize(x)).ToArray();

            if (arguments.Length != 2)
            {
                throw new InvalidOperationException("Invalid generic type argument count was " +
                    "given to serialized lambda type. (Func<InT, OutT>).");
            }

            return typeof(Func<,>).MakeGenericType(arguments);
        });
    }

    protected virtual void Init()
    {

    }
}

public class TypeSerializer
{
    public class Options
    {
        public bool UseAssemblyName { get; set; }
        public bool UseFullName { get; set; }
        public TypeMapper? Mapper { get; set; }
    }

    Options options;
    TypeMapper mapper;

    public TypeSerializer(Options? options = null)
    {
        options = options ?? DefaultOptions();
        this.options = options;
        mapper = options.Mapper ?? new TypeMapper();

        mapper.SetTypeSerializer(this);
    }

    public virtual SerializableType Serialize(Type type)
    {
        return new SerializableType()
        {
            IsGeneric = type.IsGenericTypeDefinition,
            Name = type.Name,
            Namespace = type.Namespace,
            AssemblyQualifiedName = type.AssemblyQualifiedName,
            GenericTypeArguments = type.GenericTypeArguments.Transform(x => Serialize(x)).ToArray(),
        };
    }

    public virtual Type Deserialize(SerializableType serializedType)
    {
        AppDomain domain = AppDomain.CurrentDomain;
        Assembly[] assemblies = domain.GetAssemblies();
        List<Type> types = new List<Type>();
        Type? deserializedType = null;

        foreach (Assembly assembly in assemblies)
        {
            types.AddRange(assembly.GetTypes());
        }

        var mapperValue = mapper.Get(serializedType);
        var isDeserializable = serializedType.FullNameIsAvailable() || serializedType.AssemblyNameIsAvailable();

        if (mapperValue != null)
        {
            return mapperValue;
        }

        if (!isDeserializable)
        {
            throw new InvalidOperationException($"The serialized type does not have enough information to be deserialized. '{serializedType.GetFullName()}'.");
        }

        if (deserializedType == null && serializedType.AssemblyNameIsAvailable() && options.UseAssemblyName)
        {
            deserializedType = Type.GetType(serializedType.AssemblyQualifiedName!);
        }

        if (deserializedType == null && serializedType.FullNameIsAvailable() && options.UseFullName)
        {
            deserializedType = Type.GetType(serializedType.GetFullName());
        }

        if (deserializedType == null)
        {
            throw new InvalidOperationException($"Could not find the serialized type in the current assembly '{serializedType.GetFullName()}'.");
        }

        if (serializedType.IsGeneric && serializedType.ContainsGenericArguments())
        {
            deserializedType = deserializedType.MakeGenericType(serializedType.GenericTypeArguments.Transform(x => Deserialize(x)).ToArray());
        }

        return deserializedType;
    }

    public static Options DefaultOptions()
    {
        return new Options()
        {
            UseAssemblyName = false,
            UseFullName = true,
        };
    }
}