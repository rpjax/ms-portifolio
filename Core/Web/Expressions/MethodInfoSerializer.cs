using ModularSystem.Core;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

public class SerializedMethodInfo
{
    public bool IsGeneric { get; set; }
    public string? Name { get; set; }
    public SerializedType? DeclaringType { get; set; }
    public SerializedType? ReturnType { get; set; }
    public List<SerializedType> GenericArguments { get; set; } = new();

    /// <summary>
    /// Type.Fullname + Name
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public string GetFullName()
    {
        if (DeclaringType == null)
        {
            throw new InvalidOperationException();
        }
        if (Name == null)
        {
            throw new InvalidOperationException();
        }

        return $"{DeclaringType.FullName()}.{Name}";
    }
}

public class MethodInfoMapper : IMapper<SerializedMethodInfo, MethodInfo>
{
    TypeSerializer typeSerializer;
    Dictionary<string, MethodInfo> map { get; } = new();

    public MethodInfoMapper(TypeSerializer typeSerializer)
    {
        this.typeSerializer = typeSerializer;
        PopulateMap();
        Init();
    }

    public MethodInfo? Get(SerializedMethodInfo serializedMethodInfo)
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

    public void Map(SerializedMethodInfo source, MethodInfo target)
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
        public IMapper<SerializedMethodInfo, MethodInfo>? Mapper { get; set; }
    }

    TypeSerializer typeSerializer;
    IMapper<SerializedMethodInfo, MethodInfo> mapper;

    public MethodInfoSerializer(TypeSerializer serializer, Options? options = null)
    {
        options ??= new Options();
        typeSerializer = serializer;
        mapper = options.Mapper ?? new MethodInfoMapper(typeSerializer);
    }

    public SerializedMethodInfo Serialize(MethodInfo methodInfo)
    {
        if (methodInfo.DeclaringType == null)
        {
            throw new InvalidOperationException("Could not serialize MethodInfo because its declaring type is null.");
        }

        return new SerializedMethodInfo()
        {
            IsGeneric = methodInfo.IsGenericMethod,
            Name = methodInfo.Name,
            DeclaringType = typeSerializer.Serialize(methodInfo.DeclaringType),
            ReturnType = typeSerializer.Serialize(methodInfo.ReturnType),
            GenericArguments = methodInfo.GetGenericArguments().ToList().ConvertAll(x => typeSerializer.Serialize(x))
        };
    }

    public MethodInfo Deserialize(SerializedMethodInfo serializedMethodInfo)
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