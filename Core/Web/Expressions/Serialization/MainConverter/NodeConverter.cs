using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <inheritdoc/>
public interface IExpressionConverter : IConverter<Expression, SerializableExpression>
{

}

/// <inheritdoc/>
public class NodeConverter : IExpressionConverter
{
    private Configs Config { get; }
    private ExpressionToSerializable ExpressionToNodeConversion { get; }
    private SerializableToExpression NodeToExpressionConversion { get; }

    public NodeConverter(Configs? configs = null)
    {
        Config = configs ?? new();
        ExpressionToNodeConversion = GetExpressionToNodeConversion(Config);
        NodeToExpressionConversion = GetNodeToExpressionConversion(Config);
    }

    /// <inheritdoc/>
    public SerializableExpression Convert(Expression instance)
    {
        return ExpressionToNodeConversion.Convert(instance);
    }

    /// <inheritdoc/>
    public Expression Convert(SerializableExpression instance)
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
        return new ExprToUtf8Serializer();
    }

    private ExpressionToSerializable GetExpressionToNodeConversion(Configs config)
    {
        return new(new()
        {
            TypeConverter = config.TypeConverter,
            MethodInfoConverter = config.MethodInfoConverter,
            MemberInfoConverter = config.MemberInfoConverter,
            Serializer = config.Serializer,
        });
    }

    private SerializableToExpression GetNodeToExpressionConversion(Configs config)
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
