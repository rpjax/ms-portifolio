using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Exceptions;

namespace Webql.Translation.Linq.Translators;

public static class ExpressionTranslator
{
    public static Expression TranslateExpression(TranslationContext context, WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Literal:
                return TranslateLiteralExpression(context, (WebqlLiteralExpression)node);

            case WebqlExpressionType.Reference:
                return TranslateReferenceExpression(context, (WebqlReferenceExpression)node);

            case WebqlExpressionType.ScopeAccess:
                return TranslateScopeAccessExpression(context, (WebqlScopeAccessExpression)node);

            case WebqlExpressionType.TemporaryDeclaration:
                return TranslateTemporaryDeclarationExpression(context, (WebqlTemporaryDeclarationExpression)node);

            case WebqlExpressionType.Block:
                return TranslateBlockExpression(context, (WebqlBlockExpression)node);

            case WebqlExpressionType.Operation:
                return OperationExpressionTranslator.TranslateOperationExpression(context, (WebqlOperationExpression)node);

            default:
                throw new TranslationException("Unknown expression type", context);
        }
    }

    public static Expression TranslateLiteralExpression(TranslationContext context, WebqlLiteralExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateReferenceExpression(TranslationContext context, WebqlReferenceExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateScopeAccessExpression(TranslationContext context, WebqlScopeAccessExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateTemporaryDeclarationExpression(TranslationContext context, WebqlTemporaryDeclarationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateBlockExpression(TranslationContext context, WebqlBlockExpression node)
    {
        throw new NotImplementedException();
    }

}
