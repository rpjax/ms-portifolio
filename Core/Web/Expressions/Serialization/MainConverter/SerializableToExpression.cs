using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

internal class SerializableToExpression : IConversion<SerializableExpression, Expression>
{
    private Configs Config { get; }
    private ITypeConverter TypeConverter => Config.TypeConverter;

    public SerializableToExpression(Configs? config = null)
    {
        Config = config ?? new();  
    }

    public Expression Convert(SerializableExpression node)
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
