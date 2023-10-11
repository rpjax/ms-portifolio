using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

internal class SerializableToExpression : Parser, IConversion<SerializableExpression, Expression>
{
    protected override ParsingContext Context { get; }

    private ITypeConverter TypeConverter => Context.GetDependency<ITypeConverter>();

    public SerializableToExpression(ParsingContext parentContext)
    {
        Context = parentContext.CreateChild("Serializable To Expression Conversion");
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

}
