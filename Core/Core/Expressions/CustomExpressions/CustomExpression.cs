using System.Linq.Expressions;

namespace ModularSystem.Core.Expressions;

/// <summary>
/// Represents a custom expression that is distinct from the standard system-defined expressions.
/// This abstraction facilitates specialized handling of custom expressions in various scenarios.
/// For instance, it can be used to differentiate and exclude custom expressions from certain operations:
/// <code>
/// if(expr is not CustomExpression) 
/// {
///     // Handle system-defined expressions
/// }
/// </code>
/// This class is designed to work seamlessly with the <see cref="CustomExpressionVisitor"/>, 
/// allowing custom expressions to be visited and processed in a specialized manner.
/// </summary>
public abstract class CustomExpression : Expression
{
    /// <summary>
    /// Gets the static type of the expression that this <see cref="CustomExpression"/> represents.
    /// </summary>
    public abstract override Type Type { get; }

    /// <summary>
    /// Gets the node type of this expression. This provides a way to quickly check the type of the expression without using reflection.
    /// </summary>
    public abstract override ExpressionType NodeType { get; }

    /// <summary>
    /// Accepts a visitor for this custom expression. This method is intended to be overridden by derived classes 
    /// to provide custom handling for specific visitors, especially the <see cref="CustomExpressionVisitor"/>.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    /// <returns>The result of visiting this expression.</returns>
    protected abstract override Expression Accept(ExpressionVisitor visitor);
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
