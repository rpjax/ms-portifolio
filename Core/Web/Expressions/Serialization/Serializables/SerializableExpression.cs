using System.Linq.Expressions;

namespace ModularSystem.Web.Expressions;

public abstract class SerializableExpression
{
    /// <summary>
    /// Gets or sets the specific type of the node within the expression tree, like Add, Subtract, And, Or, etc.
    /// </summary>
    public ExpressionType NodeType { get; set; }

    public static Type GetConcreteType(ExtendedExpressionType type)
    {
        switch (type)
        {
            case ExtendedExpressionType.Add:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.AddChecked:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.And:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.AndAlso:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.ArrayLength:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.ArrayIndex:
                return typeof(SerializableMethodCallExpression);
            case ExtendedExpressionType.Call:
                return typeof(SerializableMethodCallExpression);
            case ExtendedExpressionType.Coalesce:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Conditional:
                return typeof(SerializableConditionalExpression);
            case ExtendedExpressionType.Constant:
                return typeof(SerializableConstantExpression);
            case ExtendedExpressionType.Convert:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.ConvertChecked:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.Divide:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Equal:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.ExclusiveOr:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.GreaterThan:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.GreaterThanOrEqual:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Invoke:
                return typeof(SerializableInvocationExpression);
            case ExtendedExpressionType.Lambda:
                return typeof(SerializableLambdaExpression);
            case ExtendedExpressionType.LeftShift:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.LessThan:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.LessThanOrEqual:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.ListInit:
                return typeof(SerializableListInitExpression);
            case ExtendedExpressionType.MemberAccess:
                return typeof(SerializableMemberExpression);
            case ExtendedExpressionType.MemberInit:
                return typeof(SerializableMemberInitExpression);
            case ExtendedExpressionType.Modulo:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Multiply:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.MultiplyChecked:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Negate:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.UnaryPlus:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.NegateChecked:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.New:
                return typeof(SerializableNewExpression);
            case ExtendedExpressionType.NewArrayInit:
                return typeof(SerializableNewArrayExpression);
            case ExtendedExpressionType.NewArrayBounds:
                return typeof(SerializableNewArrayExpression);
            case ExtendedExpressionType.Not:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.NotEqual:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Or:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.OrElse:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Parameter:
                return typeof(SerializableParameterExpression);
            case ExtendedExpressionType.Power:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Quote:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.RightShift:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Subtract:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.SubtractChecked:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.TypeAs:
                return typeof(SerializableTypeBinaryExpression);
            case ExtendedExpressionType.TypeIs:
                return typeof(SerializableTypeBinaryExpression);
            case ExtendedExpressionType.Assign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.Block:
                return typeof(SerializableBlockExpression);
            case ExtendedExpressionType.DebugInfo:
                return typeof(SerializableDebugInfoExpression);
            case ExtendedExpressionType.Decrement:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.Dynamic:
                return typeof(SerializableDynamicExpression);
            case ExtendedExpressionType.Default:
                return typeof(SerializableDefaultExpression);
            case ExtendedExpressionType.Extension:
                return typeof(SerializableExtensionExpression);
            case ExtendedExpressionType.Goto:
                return typeof(SerializableGotoExpression);
            case ExtendedExpressionType.Increment:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.Index:
                return typeof(SerializableIndexExpression);
            case ExtendedExpressionType.Label:
                return typeof(SerializableLabelExpression);
            case ExtendedExpressionType.RuntimeVariables:
                return typeof(SerializableRuntimeVariablesExpression);
            case ExtendedExpressionType.Loop:
                return typeof(SerializableLoopExpression);
            case ExtendedExpressionType.Switch:
                return typeof(SerializableSwitchExpression);
            case ExtendedExpressionType.Throw:
                return typeof(SerializableThrowExpression);
            case ExtendedExpressionType.Try:
                return typeof(SerializableTryExpression);
            case ExtendedExpressionType.Unbox:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.AddAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.AndAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.DivideAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.ExclusiveOrAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.LeftShiftAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.ModuloAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.MultiplyAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.OrAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.PowerAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.RightShiftAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.SubtractAssign:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.AddAssignChecked:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.MultiplyAssignChecked:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.SubtractAssignChecked:
                return typeof(SerializableBinaryExpression);
            case ExtendedExpressionType.PreIncrementAssign:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.PreDecrementAssign:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.PostIncrementAssign:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.PostDecrementAssign:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.TypeEqual:
                return typeof(SerializableTypeBinaryExpression);
            case ExtendedExpressionType.OnesComplement:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.IsTrue:
                return typeof(SerializableUnaryExpression);
            case ExtendedExpressionType.IsFalse:
                return typeof(SerializableUnaryExpression);
            default:
                throw new Exception("Invalid or not supported node type.");
        }
    }

    public static Type GetConcreteType(ExpressionType type)
    {
        return GetConcreteType((ExtendedExpressionType)type);
    }
}

/// <summary>
/// Used internally by the library's default implementation to <see cref="QueryProtocol.ExpressionSerializer"/> to read the <see cref="SerializableExpression.NodeType"/> from a JSON string.
/// </summary>
internal class EmptySerializableNode : SerializableExpression
{

}

public class SerializableBinaryExpression : SerializableExpression
{
    public bool IsLiftedToNull { get; set; }
    public SerializableType? Type { get; set; }
    public SerializableExpression Left { get; set; }
    public SerializableExpression Right { get; set; }
}

public class SerializableUnaryExpression : SerializableExpression
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
    public SerializableExpression? Operand { get; set; }
}

public class SerializableMethodCallExpression : SerializableExpression
{
    public SerializableMethodInfo? MethodInfo { get; set; }
    public SerializableExpression? Target { get; set; }
    public SerializableExpression[] Arguments { get; set; } = Array.Empty<SerializableExpression>();
}

public class SerializableConditionalExpression : SerializableExpression
{
    public SerializableExpression? Test { get; set; }
    public SerializableExpression? IfTrue { get; set; }
    public SerializableExpression? IfFalse { get; set; }
}

/// <summary>
/// Represents a serializable version of <see cref="ConstantExpression"/>.
/// </summary>
public class SerializableConstantExpression : SerializableExpression
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

public class SerializableInvocationExpression : SerializableExpression
{
    public SerializableExpression? Expression { get; set; }
    public SerializableExpression[] Arguments { get; set; } = Array.Empty<SerializableExpression>();
}

public class SerializableLambdaExpression : SerializableExpression
{
    public SerializableType? Type { get; set; }
    public SerializableType? ReturnType { get; set; }
    public SerializableExpression[] Parameters { get; set; } = Array.Empty<SerializableExpression>();
    public SerializableExpression? Body { get; set; }
}

public class SerializableListInitExpression : SerializableExpression
{
    public SerializableExpression? NewExpression { get; set; }
    public SerializableElementInit[] Initializers { get; set; } = Array.Empty<SerializableElementInit>();
}

public class SerializableMemberExpression : SerializableExpression
{
    public SerializableType? Type { get; set; }
    public SerializableMemberInfo? MemberInfo { get; set; }
    public SerializableExpression? Expression { get; set; }
}

public class SerializableMemberInitExpression : SerializableExpression
{
    public SerializableExpression? NewExpression { get; set; }
    public SerializableMemberBinding[] Bindings { get; set; }
}

public class SerializableNewExpression : SerializableExpression
{
    public SerializableConstructorInfo? ConstructorInfo { get; set; }
}

public class SerializableNewArrayExpression : SerializableExpression
{
    public SerializableType? Type { get; set; }
    public SerializableExpression[] Initializers { get; set; } = Array.Empty<SerializableExpression>();
}

public class SerializableParameterExpression : SerializableExpression
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

public class SerializableTypeBinaryExpression : SerializableExpression
{
    public SerializableExpression? Expression { get; set; }
    public SerializableType? Type { get; set; }
}

public class SerializableBlockExpression : SerializableExpression
{
    public SerializableExpression[] Expressions { get; set; } = Array.Empty<SerializableExpression>();
}

public class SerializableDebugInfoExpression : SerializableExpression
{
    public SerializableSymbolDocumentInfo? SymbolDocumentInfo { get; set; }
    public int StartLine { get; set; }
    public int StartColumn { get; set; }
    public int EndLine { get; set; }
    public int EndColumn { get; set; }
}

public class SerializableDynamicExpression : SerializableExpression
{
  
}

public class SerializableDefaultExpression : SerializableExpression
{
    public SerializableType? Type { get; set; }
}

public class SerializableExtensionExpression : SerializableExpression
{

}

public class SerializableGotoExpression : SerializableExpression
{

}

public class SerializableIndexExpression : SerializableExpression
{
    public SerializableExpression? Object { get; set; }
    public SerializablePropertyInfo? Indexer { get; set; }
    public SerializableExpression[] Arguments { get; set; } = Array.Empty<SerializableExpression>();
}

public class SerializableLabelExpression : SerializableExpression
{

}

public class SerializableRuntimeVariablesExpression : SerializableExpression
{

}

public class SerializableLoopExpression : SerializableExpression
{

}

public class SerializableSwitchExpression : SerializableExpression
{

}

public class SerializableThrowExpression : SerializableExpression
{

}

public class SerializableTryExpression : SerializableExpression
{

}
