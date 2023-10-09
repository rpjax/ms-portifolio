using ModularSystem.Core;
using System.Linq.Expressions;
using System.Reflection;

namespace ModularSystem.Web.Expressions;

public interface INodeConverter : IConverter<Expression, SerializableNode>
{

}

public class NodeConverter : INodeConverter
{
    private Configs Config { get; }
    private ExpressionToNodeConversion ExpressionToNodeConversion { get; }
    private NodeToExpressionConversion NodeToExpressionConversion { get; }

    public NodeConverter(Configs? configs = null)
    {
        Config = configs ?? new();
        ExpressionToNodeConversion = GetExpressionToNodeConversion(Config);
        NodeToExpressionConversion = GetNodeToExpressionConversion(Config);
    }

    public SerializableNode Convert(Expression instance)
    {
        return ExpressionToNodeConversion.Convert(instance);
    }

    public Expression Convert(SerializableNode instance)
    {
        return NodeToExpressionConversion.Convert(instance);
    }

    static ITypeConverter DefaultTypeConverter()
    {
        return new TypeConverter();
    }

    static IMethodInfoConverter DefaultMethodInfoConverter()
    {
        return new MethodInfoConverter();
    }

    static IMemberInfoConverter DefaultMemberInfoConverter()
    {
        return new MemberInfoConverter();
    }

    static ISerializer DefaultSerializer()
    {
        return new NodeSerializer();
    }

    private ExpressionToNodeConversion GetExpressionToNodeConversion(Configs config)
    {
        return new(new()
        {
            TypeConverter = config.TypeConverter,
            MethodInfoConverter = config.MethodInfoConverter,
            MemberInfoConverter = config.MemberInfoConverter,
            Serializer = config.Serializer,
        });
    }

    private NodeToExpressionConversion GetNodeToExpressionConversion(Configs config)
    {
        return new(new()
        {
            TypeConverter = config.TypeConverter,
            MethodInfoConverter = config.MethodInfoConverter,
            MemberInfoConverter = config.MemberInfoConverter,
            Serializer = config.Serializer,
        });
    }

    public class Configs
    {
        public ITypeConverter TypeConverter { get; set; } = DefaultTypeConverter();
        public IMethodInfoConverter MethodInfoConverter { get; set; } = DefaultMethodInfoConverter();
        public IMemberInfoConverter MemberInfoConverter { get; set; } = DefaultMemberInfoConverter();
        public ISerializer Serializer { get; set; } = DefaultSerializer();
    }
}

internal class ExpressionToNodeConversion : IConversion<Expression, SerializableNode>
{
    private Configs Config { get; }
    private ITypeConverter TypeConverter => Config.TypeConverter;
    private IMethodInfoConverter MethodInfoConverter => Config.MethodInfoConverter;
    private IMemberInfoConverter MemberInfoConverter => Config.MemberInfoConverter;
    private ISerializer Serializer => Config.Serializer;

    public ExpressionToNodeConversion(Configs? config = null)
    {
        Config = config ?? new();
    }

    public SerializableNode Convert(Expression expression)
    {
        var nodeType = expression.NodeType;

        switch (nodeType)
        {
            case ExpressionType.Add: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.AddChecked: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.And: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.AndAlso: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.ArrayLength: return Convert(As<UnaryExpression>(expression));
            case ExpressionType.ArrayIndex: return Convert(As<MethodCallExpression>(expression));
            case ExpressionType.Call: return Convert(As<MethodCallExpression>(expression));
            case ExpressionType.Coalesce: return Convert(As<BinaryExpression>(expression));
            case ExpressionType.Conditional: return Convert(As<ConditionalExpression>(expression));
            case ExpressionType.Constant: return Convert(As<ConstantExpression>(expression));
            default:
                throw new Exception();
        }
    }

    private T As<T>(Expression expression) where T : Expression
    {
        return expression.TypeCast<T>();
    }

    private SerializableBinaryExpression Convert(BinaryExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            IsLiftedToNull = expression.IsLiftedToNull,
            Left = Convert(expression.Left),
            Right = Convert(expression.Right)
        };
    }

    private SerializableUnaryExpression Convert(UnaryExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            IsLiftedToNull = expression.IsLiftedToNull,
            Operand = Convert(expression.Operand)
        };
    }

    private SerializableMethodCallExpression Convert(MethodCallExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            MethodInfo = MethodInfoConverter.Convert(expression.Method),
            Arguments = expression.Arguments.Transform(x => Convert(x)).ToArray()
        };
    }

    private SerializableConditionalExpression Convert(ConditionalExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Test = Convert(expression.Test),
            IfTrue = Convert(expression.IfTrue),
            IfFalse = Convert(expression.IfFalse)
        };
    }

    private SerializableConstantExpression Convert(ConstantExpression expression)
    {
        return new()
        {
            NodeType = expression.NodeType,
            Type = TypeConverter.Convert(expression.Type),
            Value = Serializer.Serialize(expression.Value)
        };
    }

    public class Configs
    {
        public ITypeConverter TypeConverter { get; set; }
        public IMethodInfoConverter MethodInfoConverter { get; set; }
        public IMemberInfoConverter MemberInfoConverter { get; set; }
        public ISerializer Serializer { get; set; }
    }
}

internal class NodeToExpressionConversion : IConversion<SerializableNode, Expression>
{
    private Configs Config { get; }
    private ITypeConverter TypeConverter => Config.TypeConverter;

    public NodeToExpressionConversion(Configs? config = null)
    {
        Config = config ?? new();  
    }

    public Expression Convert(SerializableNode node)
    {
        var nodeType = node.NodeType;

        switch (nodeType)
        {
            default:
                throw new Exception();
        }
    }

    public class Configs
    {
        public ITypeConverter TypeConverter { get; set; }
        public IMethodInfoConverter MethodInfoConverter { get; set; }
        public IMemberInfoConverter MemberInfoConverter { get; set; }
        public ISerializer Serializer { get; set; }
    }
}

//*
// to be removed to a separeted file
//

public interface ITypeConverter : IConverter<Type, SerializableType>
{

}

public class TypeConverter : ITypeConverter
{
    private Configs Config { get; }

    public TypeConverter(Configs? config = null)
    {
        Config = config ?? new();
    }

    public SerializableType Convert(Type type)
    {
        return new SerializableType()
        {
            IsGeneric = type.IsGenericTypeDefinition,
            Name = type.Name,
            Namespace = type.Namespace,
            AssemblyQualifiedName = type.AssemblyQualifiedName,
            GenericTypeArguments = type.GenericTypeArguments.Transform(x => Convert(x)).ToArray(),
        };
    }

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
            throw new InvalidOperationException($"The serialized type does not have enough information to be deserialized. '{serializableType.FullName()}'.");
        }

        if (deserializedType == null && serializableType.AssemblyNameIsAvailable() && Config.UseAssemblyName)
        {
            deserializedType = Type.GetType(serializableType.AssemblyQualifiedName!);
        }

        if (deserializedType == null && serializableType.FullNameIsAvailable() && Config.UseFullName)
        {
            deserializedType = Type.GetType(serializableType.FullName());
        }

        if (deserializedType == null)
        {
            throw new InvalidOperationException($"Could not find the serialized type in the current assembly '{serializableType.FullName()}'.");
        }

        if (serializableType.IsGeneric && serializableType.ContainsGenericArguments())
        {
            deserializedType = deserializedType.MakeGenericType(serializableType.GenericTypeArguments.Transform(x => Convert(x)).ToArray());
        }

        return deserializedType;
    }

    public class Configs
    {
        public bool UseAssemblyName { get; set; }
        public bool UseFullName { get; set; }
    }
}

//*
// to be removed to a separeted file
//

public interface IMethodInfoConverter : IConverter<MethodInfo, SerializableMethodInfo>
{

}

public class MethodInfoConverter : IMethodInfoConverter
{
    public SerializableMethodInfo Convert(MethodInfo instance)
    {
        throw new NotImplementedException();
    }

    public MethodInfo Convert(SerializableMethodInfo instance)
    {
        throw new NotImplementedException();
    }
}

//*
// to be removed to a separeted file
//

public interface IMemberInfoConverter : IConverter<MemberInfo, SerializableMemberInfo>
{

}

public class MemberInfoConverter : IMemberInfoConverter
{
    public SerializableMemberInfo Convert(MemberInfo instance)
    {
        throw new NotImplementedException();
    }

    public MemberInfo Convert(SerializableMemberInfo instance)
    {
        throw new NotImplementedException();
    }
}