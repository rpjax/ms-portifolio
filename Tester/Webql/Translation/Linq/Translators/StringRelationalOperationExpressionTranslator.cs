using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;

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
        throw new NotImplementedException();
    }

    public static Expression TranslateRegexMatchExpression(WebqlOperationExpression node)
    {
        throw new NotImplementedException();
    }
}



