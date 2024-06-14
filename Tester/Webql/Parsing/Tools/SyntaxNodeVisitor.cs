using System.Diagnostics.CodeAnalysis;
using Webql.Parsing.Components;

namespace Webql.Parsing.Tools;

/// <summary>
/// Represents a visitor that traverses the syntax tree and performs an action on each node. <br/>
/// It dynamically dispatches the call to the appropriate method based on the node type. <br/>
/// Custom visitors can inherit from this class and override the methods to perform the desired action.
/// </summary>
public class SyntaxNodeVisitor
{
    /// <summary>
    /// Visits the specified syntax node.
    /// </summary>
    /// <param name="node">The syntax node to visit.</param>
    /// <returns>The visited syntax node.</returns>
    [return: NotNullIfNotNull("node")]
    public virtual WebqlSyntaxNode? Visit(WebqlSyntaxNode? node)
    {
        return node?.Accept(this);
    }

    /// <summary>
    /// Visits the specified query.
    /// </summary>
    /// <param name="query">The query to visit.</param>
    /// <returns>The visited query.</returns>
    public virtual WebqlQuery VisitQuery(WebqlQuery query)
    {
        Visit(query.Expression);
        return query;
    }

    /// <summary>
    /// Visits the specified literal expression.
    /// </summary>
    /// <param name="literalExpression">The literal expression to visit.</param>
    /// <returns>The visited literal expression.</returns>
    public virtual WebqlExpression VisitLiteralExpression(WebqlLiteralExpression literalExpression)
    {
        return literalExpression;
    }

    /// <summary>
    /// Visits the specified reference expression.
    /// </summary>
    /// <param name="referenceExpression">The reference expression to visit.</param>
    /// <returns>The visited reference expression.</returns>
    public virtual WebqlExpression VisitReferenceExpression(WebqlReferenceExpression referenceExpression)
    {
        return referenceExpression;
    }

    /// <summary>
    /// Visits the specified scope access expression.
    /// </summary>
    /// <param name="scopeAccessExpression">The scope access expression to visit.</param>
    /// <returns>The visited scope access expression.</returns>
    public virtual WebqlExpression VisitScopeAccessExpression(WebqlScopeAccessExpression scopeAccessExpression)
    {
        Visit(scopeAccessExpression.Expression);
        return scopeAccessExpression;
    }

    /// <summary>
    /// Visits the specified temporary declaration expression.
    /// </summary>
    /// <param name="temporaryDeclarationExpression">The temporary declaration expression to visit.</param>
    /// <returns>The visited temporary declaration expression.</returns>
    public virtual WebqlExpression VisitTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression temporaryDeclarationExpression)
    {
        Visit(temporaryDeclarationExpression.Value);
        return temporaryDeclarationExpression;
    }

    /// <summary>
    /// Visits the specified block expression.
    /// </summary>
    /// <param name="blockExpression">The block expression to visit.</param>
    /// <returns>The visited block expression.</returns>
    public virtual WebqlExpression VisitBlockExpression(WebqlBlockExpression blockExpression)
    {
        foreach (var expression in blockExpression.Expressions)
        {
            Visit(expression);
        }

        return blockExpression;
    }

    /// <summary>
    /// Visits the specified operation expression.
    /// </summary>
    /// <param name="operationExpression">The operation expression to visit.</param>
    /// <returns>The visited operation expression.</returns>
    public virtual WebqlExpression VisitOperationExpression(WebqlOperationExpression operationExpression)
    {
        foreach (var operand in operationExpression.Operands)
        {
            Visit(operand);
        }

        return operationExpression;
    }

}
