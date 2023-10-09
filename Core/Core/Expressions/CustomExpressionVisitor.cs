using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Core;

/// <summary>
/// Provides a specialized visitor for traversing and potentially modifying expressions. <br/>
/// While it offers enhanced support for custom expressions derived from <see cref="CustomExpression"/>, 
/// it remains fully <br/> compatible with all standard system-defined expressions, thanks to inheriting from <see cref="ExpressionVisitor"/>.<br/>
/// This visitor is designed to work seamlessly with both custom and standard expressions, allowing for <br/>
/// specialized handling and processing of such expressions.
/// </summary>
public class CustomExpressionVisitor : ExpressionVisitor
{
    /// <summary>
    /// Gets or sets a value indicating whether expressions of type <see cref="NotVisitableExpression"/> 
    /// should be visited.<br/>
    /// If set to false, such expressions are returned as-is without being visited.
    /// </summary>
    public bool EnableNotVisitableExpression { get; set; } = false;

    /// <summary>
    /// Visits the children of the <paramref name="node"/> and returns a copy of the node with the children changed.
    /// If <see cref="EnableNotVisitableExpression"/> is set to false and the node is of type <see cref="NotVisitableExpression"/>, 
    /// the node is returned as-is.
    /// </summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>The modified expression, if it or any sub-expression was modified; otherwise, returns the original expression.</returns>
    [return: NotNullIfNotNull("node")]
    public override Expression? Visit(Expression? node)
    {
        if (!EnableNotVisitableExpression && node is NotVisitableExpression)
        {
            return node;
        }

        return base.Visit(node);
    }

    /// <summary>
    /// Visits the <see cref="OrderingExpression"/> node and returns a new node with potentially modified children.
    /// </summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>The modified expression, if it or any sub-expression was modified; otherwise, returns the original expression.</returns>
    protected internal virtual Expression VisitOrdering(OrderingExpression node)
    {
        return new OrderingExpression(node.FieldType, Visit(node.FieldSelector));
    }

    /// <summary>
    /// Visits the <see cref="UpdateSetExpression"/> node. This method will not be called if <see cref="EnableNotVisitableExpression"/> is set to false.
    /// </summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>The original expression, as <see cref="UpdateSetExpression"/> nodes are not modified by this visitor.</returns>
    protected internal virtual Expression VisitUpdateSet(UpdateSetExpression node)
    {
        return node;
    }
}
