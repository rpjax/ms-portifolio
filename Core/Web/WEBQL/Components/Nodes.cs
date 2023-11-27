using ModularSystem.Core;
using System.Text.Json.Nodes;

namespace ModularSystem.Webql;

public abstract class Node
{
    public abstract NodeType NodeType { get; }

    public T As<T>() where T : Node
    {
        return this.TypeCast<T>();
    }
}

public class LiteralNode : Node
{
    public override NodeType NodeType { get; }
    public string? Value { get; }

    public LiteralNode(string? value)
    {
        NodeType = NodeType.Literal;
        Value = value;
    }
}

public class ArrayNode : Node
{
    public override NodeType NodeType { get; }
    public Node[] Values { get; }

    public ArrayNode(IEnumerable<Node> values)
    {
        NodeType = NodeType.Array;
        Values = values.ToArray();
    }

    public ArrayNode(params Node[] values) : this(values.ToList())
    {

    }
}

public class LhsNode : Node
{
    public override NodeType NodeType { get; }
    public bool IsOperator { get; }
    public string Value { get; }

    public LhsNode(string value)
    {
        NodeType = NodeType.LeftHandSide;
        IsOperator = value.StartsWith('$');
        Value = value;
    }
}

public class RhsNode : Node
{
    public override NodeType NodeType { get; }
    public Node Value { get; }

    public RhsNode(Node value)
    {
        NodeType = NodeType.RightHandSide;
        Value = value;
    }

    public static RhsType GetRhsType(JsonNode? jsonNode)
    {
        if (jsonNode == null)
        {
            return RhsType.Literal;
        }
        if (jsonNode is JsonObject)
        {
            return RhsType.Object;
        }
        if (jsonNode is JsonArray)
        {
            return RhsType.Array;
        }
        if (jsonNode is JsonValue)
        {
            return RhsType.Literal;
        }

        return RhsType.Invalid;
    }
}

public class ExpressionNode : Node
{
    public override NodeType NodeType { get; }
    public LhsNode Lhs { get; }
    public RhsNode Rhs { get; }

    public ExpressionNode(LhsNode lhs, RhsNode rhs)
    {
        NodeType = NodeType.Expression;
        Lhs = lhs;
        Rhs = rhs;
    }
}

public class ScopeDefinitionNode : Node
{
    public override NodeType NodeType { get; }
    public ExpressionNode[] Expressions { get; }

    public ScopeDefinitionNode(IEnumerable<ExpressionNode> expressions)
    {
        NodeType = NodeType.ScopeDefinition;
        Expressions = expressions.ToArray();
    }

    public ScopeDefinitionNode(params ExpressionNode[] expressions) : this(expressions.ToList())
    {

    }

    public ExpressionNode? this[string key]
    {
        get => Expressions.Where(x => x.Lhs.Value == key).FirstOrDefault();
    }
}