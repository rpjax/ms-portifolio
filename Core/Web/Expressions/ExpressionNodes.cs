using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

/// <summary>
/// Provides a serialized representation of an <see cref="Expression"/>, acting as a base class 
/// for different specialized types of expression nodes. The class is useful for translating between
/// different forms of expressions (like JSON or other formats) and the native LINQ expression trees.
/// </summary>
public abstract class ExpressionNode
{
    /// <summary>
    /// Gets or sets the specific type of the node within the expression tree, like Add, Subtract, And, Or, etc.
    /// </summary>
    public ExpressionType NodeType { get; set; }

    /// <summary>
    /// Maps a given <see cref="ExpressionType"/> to its corresponding specialized node type.
    /// For example, it translates the ExpressionType.Add to a <see cref="BinaryNode"/>,
    /// which represents binary operations in the expression tree.
    /// </summary>
    /// <param name="expressionType">The type of the expression in the tree.</param>
    /// <returns>
    /// The <see cref="Type"/> of the specialized <see cref="ExpressionNode"/> that is used to represent 
    /// the given expression type in the serialized form.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// Thrown when there is no matching node implementation for the provided <see cref="ExpressionType"/>.
    /// </exception>
    public static Type GetNodeType(ExpressionType expressionType)
    {
        switch (expressionType)
        {
            case ExpressionType.Add:
                return typeof(BinaryNode);
            case ExpressionType.AddChecked:
                return typeof(BinaryNode);
            case ExpressionType.And:
                return typeof(BinaryNode);
            case ExpressionType.AndAlso:
                return typeof(BinaryNode);
            case ExpressionType.ArrayLength:
                break;
            case ExpressionType.ArrayIndex:
                break;
            case ExpressionType.Call:
                return typeof(MethodCallNode);
            case ExpressionType.Coalesce:
                break;
            case ExpressionType.Conditional:
                break;
            case ExpressionType.Constant:
                return typeof(ConstantNode);
            case ExpressionType.Convert:
                return typeof(UnaryNode);
            case ExpressionType.ConvertChecked:
                break;
            case ExpressionType.Divide:
                return typeof(BinaryNode);
            case ExpressionType.Equal:
                return typeof(BinaryNode);
            case ExpressionType.ExclusiveOr:
                return typeof(BinaryNode);
            case ExpressionType.GreaterThan:
                return typeof(BinaryNode);
            case ExpressionType.GreaterThanOrEqual:
                return typeof(BinaryNode);
            case ExpressionType.Invoke:
                break;
            case ExpressionType.Lambda:
                return typeof(LambdaNode);
            case ExpressionType.LeftShift:
                return typeof(BinaryNode);
            case ExpressionType.LessThan:
                return typeof(BinaryNode);
            case ExpressionType.LessThanOrEqual:
                return typeof(BinaryNode);
            case ExpressionType.ListInit:
                break;
            case ExpressionType.MemberAccess:
                return typeof(MemberAccessNode);
            case ExpressionType.MemberInit:
                break;
            case ExpressionType.Modulo:
                break;
            case ExpressionType.Multiply:
                return typeof(BinaryNode);
            case ExpressionType.MultiplyChecked:
                return typeof(BinaryNode);
            case ExpressionType.Negate:
                return typeof(UnaryNode);
            case ExpressionType.UnaryPlus:
                break;
            case ExpressionType.NegateChecked:
                return typeof(UnaryNode);
            case ExpressionType.New:
                break;
            case ExpressionType.NewArrayInit:
                break;
            case ExpressionType.NewArrayBounds:
                break;
            case ExpressionType.Not:
                return typeof(UnaryNode);
            case ExpressionType.NotEqual:
                return typeof(BinaryNode);
            case ExpressionType.Or:
                return typeof(BinaryNode);
            case ExpressionType.OrElse:
                return typeof(BinaryNode);
            case ExpressionType.Parameter:
                return typeof(ParameterNode);
            case ExpressionType.Power:
                break;
            case ExpressionType.Quote:
                break;
            case ExpressionType.RightShift:
                return typeof(BinaryNode);
            case ExpressionType.Subtract:
                return typeof(BinaryNode);
            case ExpressionType.SubtractChecked:
                return typeof(BinaryNode);
            case ExpressionType.TypeAs:
                break;
            case ExpressionType.TypeIs:
                break;
            case ExpressionType.Assign:
                break;
            case ExpressionType.Block:
                break;
            case ExpressionType.DebugInfo:
                break;
            case ExpressionType.Decrement:
                break;
            case ExpressionType.Dynamic:
                break;
            case ExpressionType.Default:
                break;
            case ExpressionType.Extension:
                break;
            case ExpressionType.Goto:
                break;
            case ExpressionType.Increment:
                break;
            case ExpressionType.Index:
                break;
            case ExpressionType.Label:
                break;
            case ExpressionType.RuntimeVariables:
                break;
            case ExpressionType.Loop:
                break;
            case ExpressionType.Switch:
                break;
            case ExpressionType.Throw:
                break;
            case ExpressionType.Try:
                break;
            case ExpressionType.Unbox:
                break;
            case ExpressionType.AddAssign:
                return typeof(BinaryNode);
            case ExpressionType.AndAssign:
                return typeof(BinaryNode);
            case ExpressionType.DivideAssign:
                return typeof(BinaryNode);
            case ExpressionType.ExclusiveOrAssign:
                return typeof(BinaryNode);
            case ExpressionType.LeftShiftAssign:
                return typeof(BinaryNode);
            case ExpressionType.ModuloAssign:
                break;
            case ExpressionType.MultiplyAssign:
                return typeof(BinaryNode);
            case ExpressionType.OrAssign:
                return typeof(BinaryNode);
            case ExpressionType.PowerAssign:
                break;
            case ExpressionType.RightShiftAssign:
                return typeof(BinaryNode);
            case ExpressionType.SubtractAssign:
                return typeof(BinaryNode);
            case ExpressionType.AddAssignChecked:
                return typeof(BinaryNode);
            case ExpressionType.MultiplyAssignChecked:
                return typeof(BinaryNode);
            case ExpressionType.SubtractAssignChecked:
                return typeof(BinaryNode);
            case ExpressionType.PreIncrementAssign:
                break;
            case ExpressionType.PreDecrementAssign:
                break;
            case ExpressionType.PostIncrementAssign:
                break;
            case ExpressionType.PostDecrementAssign:
                break;
            case ExpressionType.TypeEqual:
                break;
            case ExpressionType.OnesComplement:
                break;
            case ExpressionType.IsTrue:
                return typeof(UnaryNode);
            case ExpressionType.IsFalse:
                return typeof(UnaryNode);
            default:
                break;
        }

        throw new InvalidOperationException("Could not find a Node implementation for the provided ExpressionType.");
    }
}

internal class RawJsonNode : ExpressionNode
{

}

/// <summary>
/// Represents a serialized version of <see cref="ConstantExpression"/>.
/// Useful for converting a constant expression to a different representation.
/// </summary>
public class ConstantNode : ExpressionNode
{
    /// <summary>
    /// Gets or sets the type of the constant.
    /// </summary>
    public SerializableType? Type { get; set; }

    /// <summary>
    /// Gets or sets the value of the constant as a string.
    /// </summary>
    public string? Value { get; set; }
}

/// <summary>
/// Represents a serialized version of <see cref="ParameterExpression"/>.
/// This class is utilized for storing information about an expression parameter.
/// </summary>
public class ParameterNode : ExpressionNode
{
    /// <summary>
    /// Indicates whether the parameter is passed by reference.
    /// </summary>
    public bool IsByRef { get; set; }

    /// <summary>
    /// Gets or sets the name of the parameter.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the type of the parameter.
    /// </summary>
    public SerializableType? Type { get; set; }
}

/// <summary>
/// Represents a serialized version of a unary expression.
/// </summary>
public class UnaryNode : ExpressionNode
{
    /// <summary>
    /// Indicates whether the expression is lifted to a nullable type.
    /// </summary>
    public bool IsLiftedToNull { get; set; }

    /// <summary>
    /// Gets or sets the type of the unary operation.
    /// </summary>
    public SerializableType? Type { get; set; }

    /// <summary>
    /// Gets or sets the operand for this unary operation.
    /// </summary>
    public ExpressionNode? Operand { get; set; }
}

/// <summary>
/// Represents a serialized version of <see cref="BinaryExpression"/>.
/// This class is utilized for storing binary operations with left and right operands.
/// </summary>
public class BinaryNode : ExpressionNode
{
    public bool IsLiftedToNull { get; set; }
    public SerializableType? Type { get; set; }
    public ExpressionNode? Left { get; set; }
    public ExpressionNode? Right { get; set; }
}

/// <summary>
/// Represents a serialized version of <see cref="MemberExpression"/>.
/// This class captures accessing a field or property of an object or class.
/// </summary>
public class MemberAccessNode : ExpressionNode
{
    public SerializableType? Type { get; set; }
    public SerializableMemberInfo? MemberInfo { get; set; }
    public ExpressionNode? Expression { get; set; }
}

/// <summary>
/// Represents a serialized version of a method call expression.
/// This class captures calling a method on an object or class.
/// </summary>
public class MethodCallNode : ExpressionNode
{
    public bool IsStatic { get; set; }
    public SerializableMethodInfo? MethodInfo { get; set; }
    public ExpressionNode? Target { get; set; }
    public List<ExpressionNode> Arguments { get; set; } = new();
}

/// <summary>
/// Represents a serialized version of a lambda expression.
/// Captures the structure of a lambda function.
/// </summary>
public class LambdaNode : ExpressionNode
{
    public SerializableType? Type { get; set; }
    public SerializableType? ReturnType { get; set; }
    public List<ParameterNode> Parameters { get; set; } = new();
    public ExpressionNode? Body { get; set; }
}