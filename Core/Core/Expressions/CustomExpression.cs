using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Represents a custom expression, distinct from system-defined expressions.<br/>
/// This differentiation allows for specific handling of custom expressions in conditional checks and logic.<br/>
/// For example, one can use this differentiation to exclude custom expressions in certain operations:
/// <code>
/// if(expr is not CustomExpression) 
/// {
///     // Handle system-defined expressions
/// }
/// </code>
/// </summary>
public abstract class CustomExpression : Expression
{
    /// <inheritdoc/>
    public abstract override Type Type { get; }

    /// <inheritdoc/>
    public abstract override ExpressionType NodeType { get; }
}

/// <summary>
/// Represents an expression that should not undergo the visiting process.<br/>
/// This can be due to various reasons such as inherent errors in the expression, 
/// or scenarios where visiting is deemed unnecessary.<br/>
/// By inheriting from this class, the expression signals to skip the visiting mechanism.<br/>
/// However, it's crucial for the implementing logic to manually verify and respect this intention,<br/>
/// as the system will not automatically skip the visitation.
/// </summary>
public abstract class NotVisitableExpression : CustomExpression
{

}
