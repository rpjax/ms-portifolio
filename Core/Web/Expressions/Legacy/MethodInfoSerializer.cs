using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

public class MethodInfoMapper : IMapper<SerializableMethodInfo, MethodInfo>
{
    TypeSerializer typeSerializer;
    Dictionary<string, MethodInfo> map { get; } = new();

    public MethodInfoMapper(TypeSerializer typeSerializer)
    {
        this.typeSerializer = typeSerializer;
        PopulateMap();
        Init();
    }

    public MethodInfo? Get(SerializableMethodInfo serializedMethodInfo)
    {
        if (serializedMethodInfo.Name == null)
        {
            return null;
        }
        if (serializedMethodInfo.DeclaringType == null)
        {
            return null;
        }

        var fullname = serializedMethodInfo.GetFullName();

        return map.Get(fullname);
    }

    public void Map(SerializableMethodInfo source, MethodInfo target)
    {
        map[source.GetFullName()] = target;
    }

    protected string GetFullName(Type type, string method)
    {
        return $"{type.FullName}.{method}";
    }

    protected void Init()
    {

    }

    void PopulateMap()
    {
        AddStringMethods();
    }

    void AddStringMethods()
    {
        var stringType = typeof(string);
        var methods = stringType.GetMethods().ToList();
        var contains = methods.Where(x => x.Name == "Contains").ToList().First();

        map.Add(GetFullName(stringType, contains.Name), contains);
    }
}

public class MethodInfoSerializer
{
    public class Options
    {
        public IMapper<SerializableMethodInfo, MethodInfo>? Mapper { get; set; }
    }

    TypeSerializer typeSerializer;
    IMapper<SerializableMethodInfo, MethodInfo> mapper;

    public MethodInfoSerializer(TypeSerializer serializer, Options? options = null)
    {
        options ??= new Options();
        typeSerializer = serializer;
        mapper = options.Mapper ?? new MethodInfoMapper(typeSerializer);
    }

    public SerializableMethodInfo Serialize(MethodInfo methodInfo)
    {
        if (methodInfo.DeclaringType == null)
        {
            throw new InvalidOperationException("Could not serialize MethodInfo because its declaring type is null.");
        }

        return new SerializableMethodInfo()
        {
            IsGeneric = methodInfo.IsGenericMethod,
            Name = methodInfo.Name,
            DeclaringType = typeSerializer.Serialize(methodInfo.DeclaringType),
            ReturnType = typeSerializer.Serialize(methodInfo.ReturnType),
            GenericArguments = methodInfo.GetGenericArguments().ToList().ConvertAll(x => typeSerializer.Serialize(x))
        };
    }

    public MethodInfo Deserialize(SerializableMethodInfo serializedMethodInfo)
    {
        if (serializedMethodInfo.Name == null)
        {
            throw new InvalidOperationException("Could not deserialize SerializedMethodInfo because its name is null.");
        }
        if (serializedMethodInfo.DeclaringType == null)
        {
            throw new InvalidOperationException("Could not deserialize SerializedMethodInfo because its declaring type is null.");
        }

        var mapperValue = mapper.Get(serializedMethodInfo);

        if (mapperValue != null)
        {
            return mapperValue;
        }

        // if the value isn't mapped, the first matching method in the type will be used.
        var type = typeSerializer.Deserialize(serializedMethodInfo.DeclaringType);
        var methods = type.GetMethods().ToList()
            .Where(x => x.Name == serializedMethodInfo.Name).ToList();

        if (methods.IsEmpty())
        {
            throw new InvalidOperationException($"Could not find the method '{serializedMethodInfo.Name}' in the type '{type.FullName}'.");
        }

        var methodInfo = methods.First();

        if (serializedMethodInfo.IsGeneric)
        {
            var args = serializedMethodInfo.GenericArguments.ConvertAll(x => typeSerializer.Deserialize(x)).ToArray();
            methodInfo.MakeGenericMethod(args);
        }

        return methodInfo;
    }
}