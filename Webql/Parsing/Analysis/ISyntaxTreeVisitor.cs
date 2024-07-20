using Webql.Parsing.Ast;

namespace Webql.Parsing.Analysis;

/// <summary>
/// Represents a visitor for the Webql syntax tree.
/// </summary>
public interface ISyntaxTreeVisitor
{
    /// <summary>
    /// Visits a Webql syntax node.
    /// </summary>
    /// <param name="node">The syntax node to visit.</param>
    /// <returns>The visited syntax node.</returns>
    WebqlSyntaxNode Visit(WebqlSyntaxNode node);

    /// <summary>
    /// Visits an anonymous object expression.
    /// </summary>
    /// <param name="node">The anonymous object expression to visit.</param>
    /// <returns>The visited anonymous object expression.</returns>
    WebqlExpression VisitAnonymousObjectExpression(WebqlAnonymousObjectExpression node);

    /// <summary>
    /// Visits an anonymous object property.
    /// </summary>
    /// <param name="node">The anonymous object property to visit.</param>
    /// <returns>The visited anonymous object property.</returns>
    WebqlAnonymousObjectProperty VisitAnonymousObjectProperty(WebqlAnonymousObjectProperty node);

    /// <summary>
    /// Visits a general expression.
    /// </summary>
    /// <param name="node">The expression to visit.</param>
    /// <returns>The visited expression.</returns>
    WebqlExpression VisitExpression(WebqlExpression node);

    /// <summary>
    /// Visits a literal expression.
    /// </summary>
    /// <param name="node">The literal expression to visit.</param>
    /// <returns>The visited literal expression.</returns>
    WebqlExpression VisitLiteralExpression(WebqlLiteralExpression node);

    /// <summary>
    /// Visits a member access expression.
    /// </summary>
    /// <param name="node">The member access expression to visit.</param>
    /// <returns>The visited member access expression.</returns>
    WebqlExpression VisitMemberAccessExpression(WebqlMemberAccessExpression node);

    /// <summary>
    /// Visits an operation expression.
    /// </summary>
    /// <param name="node">The operation expression to visit.</param>
    /// <returns>The visited operation expression.</returns>
    WebqlExpression VisitOperationExpression(WebqlOperationExpression node);

    /// <summary>
    /// Visits a Webql query.
    /// </summary>
    /// <param name="node">The query to visit.</param>
    /// <returns>The visited query.</returns>
    WebqlQuery VisitQuery(WebqlQuery node);

    /// <summary>
    /// Visits a reference expression.
    /// </summary>
    /// <param name="node">The reference expression to visit.</param>
    /// <returns>The visited reference expression.</returns>
    WebqlExpression VisitReferenceExpression(WebqlReferenceExpression node);

    /// <summary>
    /// Visits a temporary declaration expression.
    /// </summary>
    /// <param name="node">The temporary declaration expression to visit.</param>
    /// <returns>The visited temporary declaration expression.</returns>
    WebqlExpression VisitTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression node);

    /// <summary>
    /// Visits a type conversion expression.
    /// </summary>
    /// <param name="node">The type conversion expression to visit.</param>
    /// <returns>The visited type conversion expression.</returns>
    WebqlExpression VisitTypeConversionExpression(WebqlTypeConversionExpression node);
}
