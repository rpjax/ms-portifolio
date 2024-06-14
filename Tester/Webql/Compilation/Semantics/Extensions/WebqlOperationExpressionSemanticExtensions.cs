using Webql.Parsing.Components;
using Webql.Semantics.Tools;

namespace Webql.Semantics.Extensions;

public static class WebqlOperationExpressionSemanticExtensions
{
    public static WebqlOperatorCategory GetOperatorCategory(this WebqlOperationExpression expression)
    {
        return WebqlOperatorClassifier.GetOperatorCategory(expression.Operator);
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

    public static WebqlOperatorArity GetWebqlOperatorArity(this WebqlOperationExpression expression)
    {
        return WebqlOperatorClassifier.GetOperatorArity(expression.Operator);
    }

    public static bool IsNullary(this WebqlOperationExpression expression)
    {
        return expression.GetWebqlOperatorArity() == WebqlOperatorArity.Nullary;
    }

    public static bool IsUnary(this WebqlOperationExpression expression)
    {
        return expression.GetWebqlOperatorArity() == WebqlOperatorArity.Unary;
    }

    public static bool IsBinary(this WebqlOperationExpression expression)
    {
        return expression.GetWebqlOperatorArity() == WebqlOperatorArity.Binary;
    }

    public static bool IsTernary(this WebqlOperationExpression expression)
    {
        return expression.GetWebqlOperatorArity() == WebqlOperatorArity.Ternary;
    }

}