using System.Diagnostics.CodeAnalysis;
using Webql.Parsing.Ast;
using Webql.Parsing.Components;

namespace Webql.Parsing.Analysis;

/// <summary>
/// Represents a visitor that traverses the syntax tree and performs an action on each node. <br/>
/// It dynamically dispatches the call to the appropriate method based on the node type. <br/>
/// Custom visitors can inherit from this class and override the methods to perform the desired action.
/// </summary>
public class SyntaxTreeVisitor
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
    /// Visits the specified literal operand.
    /// </summary>
    /// <param name="literalExpression">The literal operand to visit.</param>
    /// <returns>The visited literal operand.</returns>
    public virtual WebqlExpression VisitLiteralExpression(WebqlLiteralExpression literalExpression)
    {
        return literalExpression;
    }

    /// <summary>
    /// Visits the specified reference operand.
    /// </summary>
    /// <param name="referenceExpression">The reference operand to visit.</param>
    /// <returns>The visited reference operand.</returns>
    public virtual WebqlExpression VisitReferenceExpression(WebqlReferenceExpression referenceExpression)
    {
        return referenceExpression;
    }

    /// <summary>
    /// Visits the specified scope access operand.
    /// </summary>
    /// <param name="scopeAccessExpression">The scope access operand to visit.</param>
    /// <returns>The visited scope access operand.</returns>
    public virtual WebqlExpression VisitScopeAccessExpression(WebqlScopeAccessExpression scopeAccessExpression)
    {
        Visit(scopeAccessExpression.Expression);
        return scopeAccessExpression;
    }

    /// <summary>
    /// Visits the specified temporary declaration operand.
    /// </summary>
    /// <param name="temporaryDeclarationExpression">The temporary declaration operand to visit.</param>
    /// <returns>The visited temporary declaration operand.</returns>
    public virtual WebqlExpression VisitTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression temporaryDeclarationExpression)
    {
        Visit(temporaryDeclarationExpression.Value);
        return temporaryDeclarationExpression;
    }

    /// <summary>
    /// Visits the specified block operand.
    /// </summary>
    /// <param name="blockExpression">The block operand to visit.</param>
    /// <returns>The visited block operand.</returns>
    public virtual WebqlExpression VisitBlockExpression(WebqlBlockExpression blockExpression)
    {
        foreach (var expression in blockExpression.Expressions)
        {
            Visit(expression);
        }

        return blockExpression;
    }

    /// <summary>
    /// Visits the specified operation operand.
    /// </summary>
    /// <param name="operationExpression">The operation operand to visit.</param>
    /// <returns>The visited operation operand.</returns>
    public virtual WebqlExpression VisitOperationExpression(WebqlOperationExpression operationExpression)
    {
        foreach (var operand in operationExpression.Operands)
        {
            Visit(operand);
        }

        return operationExpression;
    }

}
