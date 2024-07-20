using System.Linq.Expressions;
using Webql.Core.Extensions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Translation.Linq.Translators;

public static class OperationExpressionTranslator
{
    public static Expression TranslateOperationExpression(WebqlOperationExpression node)
    {
        switch (node.GetOperatorCategory())
        {   
            case WebqlOperatorCategory.Arithmetic:
                return ArithmeticOperationExpressionTranslator.TranslateArithmeticOperationExpression(node);

            case WebqlOperatorCategory.Relational:
                return RelationalOperationExpressionTranslator.TranslateRelationalOperationExpression(node);

            case WebqlOperatorCategory.StringRelational:
                return StringRelationalOperationExpressionTranslator.TranslateStringRelationalOperationExpression(node);

            case WebqlOperatorCategory.Logical:
                return LogicalOperationExpressionTranslator.TranslateLogicalOperationExpression(node);

            case WebqlOperatorCategory.Semantic:
                return SemanticOperationExpressionTranslator.TranslateSemanticOperationExpression(node);

            case WebqlOperatorCategory.CollectionManipulation:
                return CollectionManipulationExpressionTranslator.TranslateCollectionManipulationExpression(node);

            case WebqlOperatorCategory.CollectionAggregation:
                return CollectionAggregationExpressionTranslator.TranslateCollectionAggregationExpression(node);

            default:
                throw new InvalidOperationException("Invalid operator category.");
        }
    }

}
