using Webql.Core.Analysis;
using Webql.Core.Extensions;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Definitions;
using Webql.Semantics.Exceptions;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

public class TypeValidatorAnalyzer : SyntaxTreeAnalyzer
{
    public TypeValidatorAnalyzer()
    {

    }

    /*
     * TODO: Implement specific analysis for each operator category. For example:
     * - Arithmetic operands must be some numeric type.
     * - Relational operands must be of the same type.
     * - String relational operands must be of type string. (regex and a string, `[a-z]+` and "hello")
     * - Logical operands must be of type bool.
     * - etc...
     */

    protected override void AnalyzeOperationExpression(WebqlOperationExpression expression)
    {
        /*
         * This ensures that the analysis starts from the leaf nodes of the syntax tree.
         */
        base.AnalyzeOperationExpression(expression);

        switch (expression.GetOperatorArity())
        {
            case WebqlOperatorArity.Unary:
                AnalyzeUnaryExpression(expression);
                break;

            case WebqlOperatorArity.Binary:
                AnalyzeBinaryExpression(expression);
                break;

            default:
                throw new InvalidOperationException("Invalid operator arity.");
        }
    }

    private void AnalyzeUnaryExpression(WebqlOperationExpression expression)
    {
        expression.EnsureOperandCount(1);

        var operandSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();

        switch (WebqlOperatorAnalyzer.GetUnaryOperator(expression.Operator))
        {
            case WebqlUnaryOperator.Not:
                if (operandSemantics.Type != typeof(bool))
                {
                    throw new SemanticException($"Type mismatch: {operandSemantics.Type} is not a boolean type.", expression);
                }
                break;

            case WebqlUnaryOperator.Count:
                if (operandSemantics.Type.IsNotQueryable())
                {
                    throw new SemanticException($"Type mismatch: {operandSemantics.Type} is not a queryable type.", expression);
                }
                break;

            case WebqlUnaryOperator.Aggregate:
                break;

            default:
                throw new InvalidOperationException("Invalid unary operator.");
        }
    }

    private void AnalyzeBinaryExpression(WebqlOperationExpression expression)
    {
        expression.EnsureOperandCount(2);

        switch (expression.GetOperatorCategory())
        {
            case WebqlOperatorCategory.Arithmetic:
            case WebqlOperatorCategory.Relational:
            case WebqlOperatorCategory.StringRelational:
            case WebqlOperatorCategory.Logical:
                AnalyzeBinaryTypeCompatibleExpression(expression);
                return;

            case WebqlOperatorCategory.Semantic:
                AnalyzeSemanticExpression(expression);
                return;

            case WebqlOperatorCategory.CollectionManipulation:
                AnalyzeCollectionManipulationExpression(expression);
                return;

            case WebqlOperatorCategory.CollectionAggregation:
                AnalyzeCollectionAggregationExpression(expression);
                return;

            default:
                throw new InvalidOperationException("Invalid operator category.");
        }
    }

    private void AnalyzeBinaryTypeCompatibleExpression(WebqlOperationExpression expression)
    {
        var lhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();
        var rhsSemantics = expression.Operands[1].GetSemantics<IExpressionSemantics>();

        if (!SemanticsTypeHelper.TypesAreCompatible(lhsSemantics.Type, rhsSemantics.Type))
        {
            throw expression.CreateOperatorIncompatibleTypeException(lhsSemantics.Type, rhsSemantics.Type);
        }
    }

    private void AnalyzeSemanticExpression(WebqlOperationExpression operationExpression)
    {

    }

    /*
     * Collection manipulation expressions
     */

    private void AnalyzeCollectionManipulationExpression(WebqlOperationExpression expression)
    {
        expression.EnsureOperandCount(2);

        var lhs = expression.Operands[0];
        var rhs = expression.Operands[1];

        var lhsSemantics = lhs.GetSemantics<IExpressionSemantics>();
        var rhsSemantics = rhs.GetSemantics<IExpressionSemantics>();

        if (lhsSemantics.Type.IsNotQueryable())
        {
            throw new SemanticException($"Type mismatch: {lhsSemantics.Type} is not a queryable type.", expression);
        }

        var operatorType = WebqlOperatorAnalyzer.GetCollectionManipulationOperator(expression.Operator);

        switch (operatorType)
        {
            case WebqlCollectionManipulationOperator.Filter:
                if (rhsSemantics.Type != typeof(bool))
                {
                    throw new SemanticException($"Type mismatch: {rhsSemantics.Type} is not a boolean type.", expression);
                }
                break;

            case WebqlCollectionManipulationOperator.Select:
                break;

            case WebqlCollectionManipulationOperator.SelectMany:
                break;

            case WebqlCollectionManipulationOperator.Limit:
                AnalyzeLimitExpression(expression);
                break;

            case WebqlCollectionManipulationOperator.Skip:
                AnalyzeSkipExpression(expression);
                break;

            default:
                throw new InvalidOperationException("Invalid collection manipulation operator.");
        }
    }

    private void AnalyzeLimitExpression(WebqlOperationExpression expression)
    {
        expression.EnsureOperandCount(2);

        var rhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (rhsSemantics.Type != typeof(int))
        {
            throw new SemanticException($"Type mismatch: {rhsSemantics.Type} is not an integer type.", expression);
        }
    }

    private void AnalyzeSkipExpression(WebqlOperationExpression expression)
    {
        expression.EnsureOperandCount(2);

        var rhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (rhsSemantics.Type != typeof(int))
        {
            throw new SemanticException($"Type mismatch: {rhsSemantics.Type} is not an integer type.", expression);
        }
    }

    /*
     * Collection aggregation expressions
     */

    private void AnalyzeCollectionAggregationExpression(WebqlOperationExpression operationExpression)
    {
        operationExpression.EnsureOperandCount(2);

        var lhs = operationExpression.Operands[0];
        var rhs = operationExpression.Operands[1];

        var lhsSemantics = lhs.GetSemantics<IExpressionSemantics>();

        lhs.EnsureIsQueryable();

        switch (operationExpression.GetCollectionAggregationOperator())
        {
            case WebqlCollectionAggregationOperator.Count:
                break;
            case WebqlCollectionAggregationOperator.Contains:
                break;
            case WebqlCollectionAggregationOperator.Index:
                break;
            case WebqlCollectionAggregationOperator.Any:
                break;
            case WebqlCollectionAggregationOperator.All:
                break;
            case WebqlCollectionAggregationOperator.Min:
                break;
            case WebqlCollectionAggregationOperator.Max:
                break;
            case WebqlCollectionAggregationOperator.Sum:
                break;
            case WebqlCollectionAggregationOperator.Average:
                break;

            default:
                throw new InvalidOperationException("Invalid collection aggregation operator.");
        }
    }

}

