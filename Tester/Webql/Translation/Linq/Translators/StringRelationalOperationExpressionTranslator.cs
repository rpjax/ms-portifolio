using System.Linq.Expressions;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Translators;

namespace Webql.Translation.Linq;

public static class StringRelationalOperationExpressionTranslator
{
    public static Expression TranslateStringRelationalOperationExpression(WebqlOperationExpression node)
    {
        switch (node.Operator)
        {
            case WebqlOperatorType.Like:
                return TranslateLikeExpression(node);

            case WebqlOperatorType.RegexMatch:
                return TranslateRegexMatchExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator type.");
        }
    }

    public static Expression TranslateLikeExpression(WebqlOperationExpression node)
    {
        var context = node.GetCompilationContext();

        var lhs = node.Operands[0];
        var rhs = node.Operands[1];

        var lhsSemantics = lhs.GetExpressionSemantics();
        var rhsSemantics = rhs.GetExpressionSemantics();

        var lhsExpression = ExpressionTranslator.TranslateExpression(lhs);
        var rhsExpression = ExpressionTranslator.TranslateExpression(rhs);

        // x.Foo.ToLower().Contains("bar".ToLower())

        var toLowerMethodInfo = typeof(string).GetMethod("ToLower", new Type[0])!;
        var containsMethodInfo = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

        var lhsToLowerExpression = Expression.Call(lhsExpression, toLowerMethodInfo);
        var rhsToLowerExpression = Expression.Call(rhsExpression, toLowerMethodInfo);

        return Expression.Call(lhsToLowerExpression, containsMethodInfo, rhsToLowerExpression);
    }

    public static Expression TranslateRegexMatchExpression(WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }
}



