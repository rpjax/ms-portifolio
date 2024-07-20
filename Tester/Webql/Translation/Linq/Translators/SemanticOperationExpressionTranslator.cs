using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Translation.Linq.Translators;

public static class SemanticOperationExpressionTranslator
{
    public static Expression TranslateSemanticOperationExpression(WebqlOperationExpression node)
    {
        switch (node.GetSemanticOperator())
        {
            case WebqlSemanticOperator.Aggregate:
                return TranslateAggregateExpression(node);

            case WebqlSemanticOperator.New:
                return TranslateNewExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator.");
        }
    }

    private static Expression TranslateAggregateExpression(WebqlOperationExpression node)
    {
        return ExpressionTranslator.TranslateExpression(node.Operands[0]);
    }

    private static Expression TranslateNewExpression(WebqlOperationExpression node)
    {
        return ExpressionTranslator.TranslateExpression(node.Operands[0]);
    }

}
