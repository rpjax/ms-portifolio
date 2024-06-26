using Webql.Parsing.Ast;

namespace Webql.Parsing.Analysis;

public class SyntaxTreeRewriter : SyntaxTreeVisitor
{
    public WebqlSyntaxNode ExecuteRewrite(WebqlSyntaxNode node)
    {
        return Visit(node);
    }

    public override WebqlQuery VisitQuery(WebqlQuery node)
    {
        if(node.Expression is null)
        {
            return node;
        }

        return new WebqlQuery(
            expression: VisitExpression(node.Expression),
            metadata: node.Metadata,
            attributes: node.Attributes
        );
    }

    public override WebqlExpression VisitLiteralExpression(WebqlLiteralExpression node)
    {
        return new WebqlLiteralExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            literalType: node.LiteralType,
            value: node.Value
        );
    }

    public override WebqlExpression VisitReferenceExpression(WebqlReferenceExpression node)
    {
        return new WebqlReferenceExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            identifier: node.Identifier
        );
    }

    public override WebqlExpression VisitScopeAccessExpression(WebqlScopeAccessExpression node)
    {
        return new WebqlScopeAccessExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            identifier: node.Identifier,
            expression: VisitExpression(node.Expression)
        );
    }

    public override WebqlExpression VisitTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression node)
    {
        return new WebqlTemporaryDeclarationExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            identifier: node.Identifier,
            type: node.Type,
            value: VisitExpression(node.Value)
        );
    }

    public override WebqlExpression VisitBlockExpression(WebqlBlockExpression node)
    {
        return new WebqlBlockExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            expressions: node.Expressions.Select(VisitExpression)
        );
    }

    public override WebqlExpression VisitOperationExpression(WebqlOperationExpression node)
    {
        return new WebqlOperationExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            @operator: node.Operator,
            operands: node.Operands.Select(VisitExpression)
        );
    }
}
