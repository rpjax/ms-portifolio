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

    public override WebqlExpression VisitMemberAccessExpression(WebqlMemberAccessExpression node)
    {
        return new WebqlMemberAccessExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            memberName: node.MemberName,
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

    public override WebqlExpression VisitTypeConversionExpression(WebqlTypeConversionExpression node)
    {
        return new WebqlTypeConversionExpression(
            metadata: node.Metadata,
            attributes: node.Attributes,
            targetType: node.TargetType,
            expression: VisitExpression(node.Expression)
        );
    }

}
