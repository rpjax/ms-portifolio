using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq.Translators;

public static class OperationExpressionTranslator
{
    public static Expression TranslateOperationExpression(TranslationContext context, WebqlOperationExpression node)
    {
        switch (node.GetOperatorCategory())
        {
            case WebqlOperatorCategory.Arithmetic:
                return ArithmeticOperationExpressionTranslator.TranslateArithmeticOperationExpression(context, node);

            case WebqlOperatorCategory.Relational:
                return RelationalOperationExpressionTranslator.TranslateRelationalOperationExpression(context, node);

            case WebqlOperatorCategory.StringRelational:
                return StringRelationalOperationExpressionTranslator.TranslateStringRelationalOperationExpression(context, node);

            case WebqlOperatorCategory.Logical:
                return LogicalOperationExpressionTranslator.TranslateLogicalOperationExpression(context, node);

            case WebqlOperatorCategory.Semantic:
                return SemanticOperationExpressionTranslator.TranslateSemanticOperationExpression(context, node);

            case WebqlOperatorCategory.CollectionManipulation:
                return CollectionManipulationExpressionTranslator.TranslateCollectionManipulationExpression(context, node);

            case WebqlOperatorCategory.CollectionAggregation:
                return CollectionAggregationExpressionTranslator.TranslateCollectionAggregationExpression(context, node);

            default:
                throw new InvalidOperationException("Invalid operator category.");
        }
    }

}
