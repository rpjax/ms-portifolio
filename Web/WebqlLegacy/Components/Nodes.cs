using ModularSystem.Core;
using System.Collections;

namespace ModularSystem.Webql;

/// <summary>
/// Represents the base class for all WebQL node types. <br/>
/// This abstract class provides the common structure and behavior for various types of nodes used in WebQL queries.
/// </summary>
public abstract class Node
{
    /// <summary>
    /// Gets the type of the node, indicating its role in a WebQL query.
    /// </summary>
    public abstract NodeType NodeType { get; }

    /// <summary>
    /// Returns a string representation of the node.
    /// </summary>
    /// <returns>A string that represents the current node.</returns>
    public abstract override string ToString();

    /// <summary>
    /// Casts the node to the specified type.
    /// </summary>
    /// <typeparam name="T">The type to cast the node to.</typeparam>
    /// <returns>The node cast to the specified type.</returns>
    public T As<T>() where T : Node
    {
        return this.TypeCast<T>();
    }
}

/// <summary>
/// Represents a literal node in a WebQL query. <br/>
/// A literal node can hold a value or represent an operator within the query.
/// </summary>
public class LiteralNode : Node
{
    /// <summary>
    /// Gets the node type as Literal.
    /// </summary>
    public override NodeType NodeType { get; }

    /// <summary>
    /// Gets the value of the literal node.
    /// </summary>
    public string? Value { get; }

    /// <summary>
    /// Indicates whether the node represents a reference operator.
    /// </summary>
    public bool IsReference { get; }

    /// <summary>
    /// Indicates whether the node does not represent a reference operator.
    /// </summary>
    public bool IsNotReference => !IsReference;

    /// <summary>
    /// Initializes a new instance of the LiteralNode class with a given value.
    /// </summary>
    /// <param name="value">The value of the literal node.</param>
    public LiteralNode(string? value)
    {
        NodeType = NodeType.Literal;
        IsReference = value?.StartsWith("\"$") == true;
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value ?? string.Empty;
    }

    public string? GetNormalizedValue()
    {
        var value = Value;

        if (value == null)
        {
            return null;
        }
        if (value.StartsWith('"'))
        {
            value = value[1..^0];
        }
        if (value.EndsWith('"'))
        {
            value = value[0..^1];
        }

        return value;
    }
}

/// <summary>
/// Represents an array node in a WebQL query, containing multiple child nodes. <br/>
/// This class allows for the representation of array structures within the query.
/// </summary>
public class ArrayNode : Node, IEnumerable<Node>
{
    /// <summary>
    /// Gets the node type as Array.
    /// </summary>
    public override NodeType NodeType { get; }

    /// <summary>
    /// Gets the array of child nodes contained in the array node.
    /// </summary>
    public Node[] Values { get; }

    /// <summary>
    /// Gets the number of child nodes in the array.
    /// </summary>
    public int Length => Values.Length;

    /// <summary>
    /// Initializes a new instance of the ArrayNode class with a collection of child nodes.
    /// </summary>
    /// <param name="values">The collection of child nodes.</param>
    public ArrayNode(IEnumerable<Node> values)
    {
        NodeType = NodeType.Array;
        Values = values.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the ArrayNode class with an array of child nodes.
    /// </summary>
    /// <param name="values">The array of child nodes.</param>
    public ArrayNode(params Node[] values) : this(values.ToList())
    {

    }

    /// <summary>
    /// Gets the child node at the specified index in the array.
    /// </summary>
    /// <param name="index">The zero-based index of the child node to get.</param>
    /// <returns>The child node at the specified index.</returns>
    public Node this[int index]
    {
        get => Values[index];
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var values = Values.Transform(value => value.ToString());
        var joinedValues = string.Join(", ", values);
        return $"[{joinedValues}]";
    }

    /// <inheritdoc/>
    public IEnumerator<Node> GetEnumerator()
    {
        return Values.AsEnumerable().GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Values.GetEnumerator();
    }
}

/// <summary>
/// Represents the left-hand side (LHS) of an expression node in a WebQL query. <br/>
/// This class holds the key or identifier part of an expression.
/// </summary>
public class LhsNode : Node
{
    /// <summary>
    /// Gets the node type as LeftHandSide.
    /// </summary>
    public override NodeType NodeType { get; }

    /// <summary>
    /// Indicates whether the node represents an operator.
    /// </summary>
    public bool IsOperator { get; }

    /// <summary>
    /// Indicates whether the node does not represent an operator.
    /// </summary>
    public bool IsNotOperator => !IsOperator;

    /// <summary>
    /// Indicates whether the node represents a symbol reference.
    /// </summary>
    public bool IsReference { get; }

    /// <summary>
    /// Indicates whether the node does not represent a symbol reference.
    /// </summary>
    public bool IsNotReference => !IsReference;

    /// <summary>
    /// Gets the value of the LHS node.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Initializes a new instance of the LhsNode class with a given value.
    /// </summary>
    /// <param name="value">The value of the LHS node.</param>
    public LhsNode(string value)
    {
        NodeType = NodeType.LeftHandSide;
        IsOperator = value.StartsWith('$');
        IsReference = !IsOperator;
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value;
    }
}

/// <summary>
/// Represents the right-hand side (RHS) of an expression node in a WebQL query. <br/>
/// This class holds the value or expression part of an expression.
/// </summary>
public class RhsNode : Node
{
    /// <summary>
    /// Gets the node type as RightHandSide.
    /// </summary>
    public override NodeType NodeType { get; }

    /// <summary>
    /// Gets the value of the RHS node.
    /// </summary>
    public Node Value { get; }

    /// <summary>
    /// Initializes a new instance of the RhsNode class with a given value.
    /// </summary>
    /// <param name="value">The value or expression of the RHS node.</param>
    public RhsNode(Node value)
    {
        NodeType = NodeType.RightHandSide;
        Value = value;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return Value.ToString();
    }
}

/// <summary>
/// Represents an expression node in a WebQL query, consisting of a left-hand side (LHS) and right-hand side (RHS). <br/>
/// This class encapsulates a key-value pair structure of an expression within the query.
/// </summary>
public class ExpressionNode : Node
{
    /// <summary>
    /// Gets the node type as Expression.
    /// </summary>
    public override NodeType NodeType { get; }

    /// <summary>
    /// Gets the LHS node of the expression.
    /// </summary>
    public LhsNode Lhs { get; }

    /// <summary>
    /// Gets the RHS node of the expression.
    /// </summary>
    public RhsNode Rhs { get; }

    /// <summary>
    /// Initializes a new instance of the ExpressionNode class with the given LHS and RHS nodes.
    /// </summary>
    /// <param name="lhs">The LHS node of the expression.</param>
    /// <param name="rhs">The RHS node of the expression.</param>
    public ExpressionNode(LhsNode lhs, RhsNode rhs)
    {
        NodeType = NodeType.Expression;
        Lhs = lhs;
        Rhs = rhs;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"\"{Lhs}\": {Rhs}";
    }
}

/// <summary>
/// Represents an object node in a WebQL query, containing multiple expression nodes <br/>
/// This class allows for the representation of complex object structures within the query.
/// </summary>
public class ObjectNode : Node, IEnumerable<ExpressionNode>
{
    /// <summary>
    /// Gets the node type as ScopeDefinition.
    /// </summary>
    public override NodeType NodeType { get; }

    /// <summary>
    /// Gets an array of expression nodes contained in the object node.
    /// </summary>
    public ExpressionNode[] Expressions { get; }

    /// <summary>
    /// Initializes a new instance of the ObjectNode class with a collection of expression nodes.
    /// </summary>
    /// <param name="expressions">The collection of expression nodes.</param>
    public ObjectNode(IEnumerable<ExpressionNode> expressions)
    {
        NodeType = NodeType.Object;
        Expressions = expressions.ToArray();
    }

    /// <summary>
    /// Initializes a new instance of the ObjectNode class with an array of expression nodes.
    /// </summary>
    /// <param name="expressions">The array of expression nodes.</param>
    public ObjectNode(params ExpressionNode[] expressions) : this(expressions.ToList())
    {

    }

    /// <summary>
    /// Gets the expression node associated with a specific key within the object node.
    /// </summary>
    /// <param name="key">The key of the expression node to get.</param>
    /// <returns>The expression node associated with the specified key, or null if not found.</returns>
    public ExpressionNode? this[string key]
    {
        get => Expressions.Where(x => x.Lhs.Value == key).FirstOrDefault();
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var values = Expressions.Transform(x => x.ToString());
        var separator = $", {Environment.NewLine}";
        var joinedValues = string.Join(separator, values);

        return $"{{ {joinedValues} }}";
    }

    /// <inheritdoc/>
    public IEnumerator<ExpressionNode> GetEnumerator()
    {
        return Expressions.AsEnumerable().GetEnumerator();
    }

    /// <inheritdoc/>
    IEnumerator IEnumerable.GetEnumerator()
    {
        return Expressions.GetEnumerator();
    }
}

public class NullNode : Node
{
    public override NodeType NodeType => NodeType.Null;

    public override string ToString()
    {
        return "null";
    }
}