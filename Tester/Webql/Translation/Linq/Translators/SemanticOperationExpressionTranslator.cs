using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq.Translators;

public static class SemanticOperationExpressionTranslator
{
    public static Expression TranslateSemanticOperationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.GetSemanticOperator())
        {
            case WebqlSemanticOperator.Aggregate:
                return ExpressionTranslator.TranslateExpression(node.Operands[0]);

            default:
                throw new InvalidOperationException("Invalid operator.");
        }
    }

}
