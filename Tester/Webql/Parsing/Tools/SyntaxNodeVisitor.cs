using System.Diagnostics.CodeAnalysis;
using Webql.DocumentSyntax.Parsing.Components;

namespace Webql.DocumentSyntax.Parsing.Tools;

/// <summary>
/// Represents a visitor that traverses the syntax tree and performs an action on each node. <br/>
/// It dynamically dispatches the call to the appropriate method based on the node type. <br/>
/// Custom visitors can inherit from this class and override the methods to perform the desired action.
/// </summary>
public class SyntaxNodeVisitor
{
    [return: NotNullIfNotNull("node")]
    public virtual WebqlSyntaxNode? Visit(WebqlSyntaxNode? node)
    {
        return node?.Accept(this);
    }

    public virtual WebqlQuery VisitQuery(WebqlQuery query)
    {
        Visit(query.Expression);
        return query;
    }

    public virtual WebqlExpression VisitLiteralExpression(WebqlLiteralExpression literalExpression)
    {
        return literalExpression;
    }

    public virtual WebqlExpression VisitReferenceExpression(WebqlReferenceExpression referenceExpression)
    {
        return referenceExpression;
    }

    public virtual WebqlExpression VisitScopeAccessExpression(WebqlScopeAccessExpression scopeAccessExpression)
    {
        Visit(scopeAccessExpression.Expression);
        return scopeAccessExpression;
    }

    public virtual WebqlExpression VisitTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression temporaryDeclarationExpression)
    {
        Visit(temporaryDeclarationExpression.Value);
        return temporaryDeclarationExpression;
    }

    public virtual WebqlExpression VisitBlockExpression(WebqlBlockExpression blockExpression)
    {
        foreach (var expression in blockExpression.Expressions)
        {
            Visit(expression);
        }

        return blockExpression;
    }

    public virtual WebqlExpression VisitOperationExpression(WebqlOperationExpression operationExpression)
    {
        foreach (var operand in operationExpression.Operands)
        {
            Visit(operand);
        }

        return operationExpression;
    }

}
