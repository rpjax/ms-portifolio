using ModularSystem.Core;
using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public class ExprSerializer : ISerializer<Expression>
{
    public SerializableExpression ToSerializable(Expression expression)
    {
        throw new NotImplementedException();
    }

    public Expression FromSerializable(SerializableExpression serializedExpression)
    {
        throw new NotImplementedException();
    }

    public string Serialize(Expression obj)
    {
        throw new NotImplementedException();
    }

    public Expression? TryDeserialize(string serialized)
    {
        throw new NotImplementedException();
    }

    public Expression Deserialize(string serialized)
    {
        throw new NotImplementedException();
    }

}
