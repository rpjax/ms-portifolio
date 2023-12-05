using ModularSystem.Core;
using System.Linq.Expressions;
using System.Text.Json.Nodes;

namespace ModularSystem.Webql;

public abstract class Node
{
    public abstract NodeType NodeType { get; }

    public abstract override string ToString();

    public T As<T>() where T : Node
    {
        return this.TypeCast<T>();
    }
}

public class LiteralNode : Node
{
    public override NodeType NodeType { get; }
    public string? Value { get; }
    public bool IsOperator { get; }

    public LiteralNode(string? value)
    {
        NodeType = NodeType.Literal;
        IsOperator = value.StartsWith("\"$");
        Value = value;
    }

    public override string ToString()
    {
        return Value ?? string.Empty;
    }
}

public class ArrayNode : Node
{
    public override NodeType NodeType { get; }
    public Node[] Values { get; }
    public int Length => Values.Length;

    public ArrayNode(IEnumerable<Node> values)
    {
        NodeType = NodeType.Array;
        Values = values.ToArray();
    }

    public ArrayNode(params Node[] values) : this(values.ToList())
    {

    }

    public Node this[int index]
    {
        get => Values[index];
    }

    public override string ToString()
    {
        var values = Values.Transform(value => value.ToString());
        var joinedValues = string.Join(", ", values);
        return $"[{joinedValues}]";
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

    public override string ToString()
    {
        return Value;
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

    public override string ToString()
    {
        return Value.ToString();
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

    public override string ToString()
    {
        return $"\"{Lhs}\": {Rhs}";
    }
}

public class ObjectNode : Node
{
    public override NodeType NodeType { get; }
    public ExpressionNode[] Expressions { get; }

    public ObjectNode(IEnumerable<ExpressionNode> expressions)
    {
        NodeType = NodeType.ScopeDefinition;
        Expressions = expressions.ToArray();
    }

    public ObjectNode(params ExpressionNode[] expressions) : this(expressions.ToList())
    {

    }

    public ExpressionNode? this[string key]
    {
        get => Expressions.Where(x => x.Lhs.Value == key).FirstOrDefault();
    }

    public override string ToString()
    {
        var values = Expressions.Transform(x => x.ToString());
        var separator = $", {Environment.NewLine}";
        var joinedValues = string.Join(separator, values);
        var str = $"{{ {joinedValues} }}";
        return $"{{ {joinedValues} }}";
    }
}