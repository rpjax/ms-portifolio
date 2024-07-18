using System.Diagnostics.CodeAnalysis;
using Webql.Parsing.Ast;

namespace Webql.Parsing.Analysis;

/// <summary>
/// Represents a visitor that traverses the syntax tree and performs an action on each node. <br/>
/// It dynamically dispatches the call to the appropriate method based on the node type. <br/>
/// Custom visitors can inherit from this class and override the methods to perform the desired action.
/// </summary>
public class SyntaxTreeVisitor : ISyntaxTreeVisitor
{
    /// <summary>
    /// Visits the specified syntax node.
    /// </summary>
    /// <param name="node">The syntax node to visit.</param>
    /// <returns>The visited syntax node.</returns>
    public virtual WebqlSyntaxNode Visit(WebqlSyntaxNode node)
    {
        return node.Accept(this);
    }

    /// <summary>
    /// Visits the specified node.
    /// </summary>
    /// <param name="node">The node to visit.</param>
    /// <returns>The visited node.</returns>
    public virtual WebqlQuery VisitQuery(WebqlQuery node)
    {
        NullableVisit(node.Expression);
        return node;
    }

    /// <summary>
    /// Visits the specified node.
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public virtual WebqlExpression VisitExpression(WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                return VisitLiteralExpression((WebqlLiteralExpression)node);

            case WebqlExpressionType.Reference:
                return VisitReferenceExpression((WebqlReferenceExpression)node);

            case WebqlExpressionType.MemberAccess:
                return VisitMemberAccessExpression((WebqlMemberAccessExpression)node);

            case WebqlExpressionType.TemporaryDeclaration:
                return VisitTemporaryDeclarationExpression((WebqlTemporaryDeclarationExpression)node);

            case WebqlExpressionType.Block:
                return VisitBlockExpression((WebqlBlockExpression)node);

            case WebqlExpressionType.Operation:
                return VisitOperationExpression((WebqlOperationExpression)node);

            case WebqlExpressionType.TypeConversion:
                return VisitTypeConversionExpression((WebqlTypeConversionExpression)node);

            case WebqlExpressionType.AnonymousObject:
                return VisitAnonymousObjectExpression((WebqlAnonymousObjectExpression)node);

            default:
                throw new InvalidOperationException("Invalid node type.");
        }
    }

    /// <summary>
    /// Visits the specified literal operand.
    /// </summary>
    /// <param name="node">The literal operand to visit.</param>
    /// <returns>The visited literal operand.</returns>
    public virtual WebqlExpression VisitLiteralExpression(WebqlLiteralExpression node)
    {
        return node;
    }

    /// <summary>
    /// Visits the specified reference operand.
    /// </summary>
    /// <param name="node">The reference operand to visit.</param>
    /// <returns>The visited reference operand.</returns>
    public virtual WebqlExpression VisitReferenceExpression(WebqlReferenceExpression node)
    {
        return node;
    }

    /// <summary>
    /// Visits the specified member access operand.
    /// </summary>
    /// <param name="node">The member access operand to visit.</param>
    /// <returns>The visited member access operand.</returns>
    public virtual WebqlExpression VisitMemberAccessExpression(WebqlMemberAccessExpression node)
    {
        Visit(node.Expression);
        return node;
    }

    /// <summary>
    /// Visits the specified temporary declaration operand.
    /// </summary>
    /// <param name="node">The temporary declaration operand to visit.</param>
    /// <returns>The visited temporary declaration operand.</returns>
    public virtual WebqlExpression VisitTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression node)
    {
        Visit(node.Value);
        return node;
    }

    /// <summary>
    /// Visits the specified block operand.
    /// </summary>
    /// <param name="node">The block operand to visit.</param>
    /// <returns>The visited block operand.</returns>
    public virtual WebqlExpression VisitBlockExpression(WebqlBlockExpression node)
    {
        foreach (var expression in node.Expressions)
        {
            Visit(expression);
        }

        return node;
    }

    /// <summary>
    /// Visits the specified operation operand.
    /// </summary>
    /// <param name="node">The operation operand to visit.</param>
    /// <returns>The visited operation operand.</returns>
    public virtual WebqlExpression VisitOperationExpression(WebqlOperationExpression node)
    {
        foreach (var operand in node.Operands)
        {
            Visit(operand);
        }

        return node;
    }

    /// <summary>
    /// Visits the specified type conversion operand.
    /// </summary>
    /// <param name="node"></param>
    /// <returns>The visited node.</returns>
    public virtual WebqlExpression VisitTypeConversionExpression(WebqlTypeConversionExpression node)
    {
        Visit(node.Expression);
        return node;
    }

    /// <summary>
    /// Visits the specified anonymous type expression.
    /// </summary>
    /// <param name="node">The anonymous type expression to visit.</param>
    /// <returns>The visited anonymous type expression.</returns>
    public virtual WebqlExpression VisitAnonymousObjectExpression(WebqlAnonymousObjectExpression node)
    {
        foreach (var property in node.Properties)
        {
            Visit(property);
        }

        return node;
    }

    /*
     * Anonymouse type property
     */

    /// <summary>
    /// Visits the specified anonymous object property.
    /// </summary>
    /// <param name="node">The anonymous type property to visit.</param>
    /// <returns>The visited anonymous type property.</returns>
    public virtual WebqlAnonymousObjectProperty VisitAnonymousObjectProperty(WebqlAnonymousObjectProperty node)
    {
        Visit(node.Value);
        return node;
    }

    /*
     * Helper methods
     */

    [return: NotNullIfNotNull("node")]
    private WebqlSyntaxNode? NullableVisit(WebqlSyntaxNode? node)
    {
        return node is not null ? Visit(node) : null;
    }

}
