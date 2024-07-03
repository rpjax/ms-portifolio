using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Exceptions;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Translators;

public static class ExpressionTranslator
{
    public static Expression TranslateExpression(WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                return LiteralExpressionTranslator.TranslateLiteralExpression((WebqlLiteralExpression)node);

            case WebqlExpressionType.Reference:
                return TranslateReferenceExpression((WebqlReferenceExpression)node);

            case WebqlExpressionType.MemberAccess:
                return TranslateMemberAccessExpression((WebqlMemberAccessExpression)node);

            case WebqlExpressionType.TemporaryDeclaration:
                return TranslateTemporaryDeclarationExpression((WebqlTemporaryDeclarationExpression)node);

            case WebqlExpressionType.Block:
                return TranslateBlockExpression((WebqlBlockExpression)node);

            case WebqlExpressionType.Operation:
                return OperationExpressionTranslator.TranslateOperationExpression((WebqlOperationExpression)node);

            default:
                throw new TranslationException("Unknown expression type", node);
        }
    }

    public static Expression TranslateReferenceExpression(WebqlReferenceExpression node)
    {
        return node.GetTranslationContext().GetExpression(node.Identifier);
    }

    public static Expression TranslateMemberAccessExpression(WebqlMemberAccessExpression node)
    {
        throw new NotImplementedException();
        return SyntaxNodeTranslator.TranslateNode(node.Expression);
    }

    public static Expression TranslateTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateBlockExpression(WebqlBlockExpression node)
    {
        var expression = null as Expression;

        foreach (var expressionNode in node.Expressions)
        {
            expression = SyntaxNodeTranslator.TranslateNode(expressionNode);
        }

        if(expression == null)
        {
            throw new TranslationException("Block expression must have at least one expression", node);
        }

        return expression;
    }

}
