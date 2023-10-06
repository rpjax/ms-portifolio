using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace ModularSystem.Core;

public class CustomExpressionVisitor : ExpressionVisitor
{
    /// <inheritdoc/>
    [return: NotNullIfNotNull("node")]
    public override Expression? Visit(Expression? node)
    {
        if(node is NotVisitableExpression)
        {
            return node;
        }

        return base.Visit(node);
    }

    protected internal virtual Expression VisitOrdering(OrderingExpression node)
    {
        return new OrderingExpression(node.FieldType, Visit(node.FieldSelector));
    }

    protected internal virtual Expression VisitUpdateSet(UpdateSetExpression node)
    {
        return node;
    }
}