using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Semantics.Definitions;
using Webql.Semantics.Extensions;
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

            case WebqlExpressionType.ScopeAccess:
                return TranslateScopeAccessExpression((WebqlScopeAccessExpression)node);

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
        throw new NotImplementedException();
    }

    public static Expression TranslateScopeAccessExpression(WebqlScopeAccessExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateBlockExpression(WebqlBlockExpression node)
    {
        throw new NotImplementedException();
    }

}

public static class LiteralExpressionTranslator
{
    public static Expression TranslateLiteralExpression(WebqlLiteralExpression node)
    {
        switch (node.LiteralType)
        {
            case WebqlLiteralType.Null:
                return TranslateNullLiteral(node);

            case WebqlLiteralType.Bool:
                return TranslateBoolLiteral(node);

            case WebqlLiteralType.Int:
                return TranslateIntLiteral(node);

            case WebqlLiteralType.Float:
                return TranslateFloatLiteral(node);

            case WebqlLiteralType.Hex:
                return TranslateHexLiteral(node);

            case WebqlLiteralType.String:
                return TranslateStringLiteral(node);

            default:
                throw new TranslationException("Unknown literal type", node);
        }
    }

    public static Expression TranslateNullLiteral(WebqlLiteralExpression node)
    {
        var semantics = node.GetSemantics<IExpressionSemantics>();
        var value = null as object;
        var type = semantics.Type;

        return Expression.Constant(value, type);
    }

    public static Expression TranslateBoolLiteral(WebqlLiteralExpression node)
    {
        var semantics = node.GetSemantics<IExpressionSemantics>();
        var value = node.Value == "true"
            ? true
            : false;

        var type = typeof(bool);

        return Expression.Constant(value, type);
    }

    public static Expression TranslateIntLiteral(WebqlLiteralExpression node)
    {
        var semantics = node.GetSemantics<IExpressionSemantics>();
        var value = int.Parse(node.Value);
        var type = typeof(int);

        return Expression.Constant(value, type);
    }

    public static Expression TranslateFloatLiteral(WebqlLiteralExpression node)
    {
        var semantics = node.GetSemantics<IExpressionSemantics>();
        var value = float.Parse(node.Value);
        var type = typeof(float);

        return Expression.Constant(value, type);
    }

    public static Expression TranslateHexLiteral(WebqlLiteralExpression node)
    {
        throw new NotImplementedException();
    }

    public static Expression TranslateStringLiteral(WebqlLiteralExpression node)
    {
        var semantics = node.GetSemantics<IExpressionSemantics>();
        var value = node.GetNormalizedStringValue();
        var type = typeof(string);

        return Expression.Constant(value, type);
    }

}
