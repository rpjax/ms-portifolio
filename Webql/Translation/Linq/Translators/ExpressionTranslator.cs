using System.Linq.Expressions;
using System.Reflection;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
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

            case WebqlExpressionType.Operation:
                return OperationExpressionTranslator.TranslateOperationExpression((WebqlOperationExpression)node);

            case WebqlExpressionType.TypeConversion:
                return TranslateTypeConversionExpression((WebqlTypeConversionExpression)node);

            case WebqlExpressionType.AnonymousObject:
                return TranslateAnonymousObjectExpression((WebqlAnonymousObjectExpression)node);

            default:
                throw new TranslationException("Unknown expression type", node);
        }
    }

    private static Expression TranslateReferenceExpression(WebqlReferenceExpression node)
    {
        return node.GetTranslationContext().GetExpression(node.Identifier);
    }

    private static Expression TranslateMemberAccessExpression(WebqlMemberAccessExpression node)
    {
        var semantics = node.GetMemberAccessSemantics();     
        var expression = TranslateExpression(node.Expression);

        return Expression.Property(expression, semantics.PropertyInfo);
    }

    private static Expression TranslateTemporaryDeclarationExpression(WebqlTemporaryDeclarationExpression node)
    {
        throw new NotImplementedException();
    }

    private static Expression TranslateTypeConversionExpression(WebqlTypeConversionExpression node)
    {
        return Expression.Convert(
            expression: TranslateExpression(node.Expression),
            type: node.TargetType
        );
    }

    private static Expression TranslateAnonymousObjectExpression(WebqlAnonymousObjectExpression node)
    {
        var semantics = node.GetAnonymousObjectSemantics();
        var objectType = semantics.Type;
        var objectTypeProperties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var objectBindings = new List<MemberBinding>();

        foreach (var property in node.Properties)
        {
            var propertySemantics = property.GetAnonymousObjectPropertySemantics();
            var propertyExpression = TranslateExpression(property.Value);

            var binding = Expression.Bind(propertySemantics.PropertyInfo, propertyExpression);

            objectBindings.Add(binding);
        }
        
        return Expression.MemberInit(Expression.New(semantics.Type), objectBindings);
    }
}
