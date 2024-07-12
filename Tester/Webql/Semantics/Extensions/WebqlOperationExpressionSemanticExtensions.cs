using Webql.Core.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Analysis;

namespace Webql.Semantics.Extensions;

public static class WebqlOperationExpressionSemanticExtensions
{
    public static WebqlOperatorCategory GetOperatorCategory(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetOperatorCategory(expression.Operator);
    }

    public static WebqlSemanticOperator GetSemanticOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetSemanticOperator(expression.Operator);
    }

    public static WebqlCollectionOperator GetCollectionOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetCollectionOperator(expression.Operator);
    }

    public static WebqlCollectionManipulationOperator GetCollectionManipulationOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetCollectionManipulationOperator(expression.Operator);
    }

    public static WebqlCollectionAggregationOperator GetCollectionAggregationOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetCollectionAggregationOperator(expression.Operator);
    }

    public static bool IsArithmetic(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.Arithmetic;
    }

    public static bool IsRelational(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.Relational;
    }

    public static bool IsStringRelational(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.StringRelational;
    }

    public static bool IsLogical(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.Logical;
    }

    public static bool IsSemantic(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.Semantic;
    }

    public static bool IsCollectionManipulation(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.CollectionManipulation;
    }

    public static bool IsCollectionAggregation(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorCategory() == WebqlOperatorCategory.CollectionAggregation;
    }

    public static WebqlOperatorArity GetOperatorArity(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.GetOperatorArity(expression.Operator);
    }

    public static bool IsUnary(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorArity() == WebqlOperatorArity.Unary;
    }

    public static bool IsBinary(this WebqlOperationExpression expression)
    {
        return expression.GetOperatorArity() == WebqlOperatorArity.Binary;
    }

    public static bool IsBinaryTypeCompatibleOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.IsBinaryTypeCompatibleOperator(expression.Operator);
    }

    public static bool IsLinqQueryableMethodCallOperator(this WebqlOperationExpression expression)
    {
        var operatorCategory = expression.GetOperatorCategory();

        return false
            || operatorCategory == WebqlOperatorCategory.CollectionManipulation
            || operatorCategory == WebqlOperatorCategory.CollectionAggregation;
    }

    public static bool IsCollectionOperator(this WebqlOperationExpression expression)
    {
        return WebqlOperatorAnalyzer.IsCollectionOperator(expression.Operator);
    }

}