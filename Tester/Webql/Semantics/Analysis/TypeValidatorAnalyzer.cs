using Webql.Core.Analysis;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Definitions;
using Webql.Semantics.Exceptions;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

public class TypeValidatorAnalyzer : SyntaxTreeAnalyzer
{
    private SemanticContext SemanticContext { get; }    
        
    public TypeValidatorAnalyzer(SemanticContext context)
    {
        SemanticContext = context;    
    }

    /*
     * TODO: Implement specific analysis for each operator category. For example:
     * - Arithmetic operands must be some numeric type.
     * - Relational operands must be of the same type.
     * - String relational operands must be of type string. (regex and a string, `[a-z]+` and "hello")
     * - Logical operands must be of type bool.
     * - etc...
     */

    protected override void AnalyzeBlockExpression(WebqlBlockExpression expression)
    {
        switch (expression.GetScopeType())
        {
            case WebqlScopeType.Aggregation:
                break;
            case WebqlScopeType.LogicalFiltering:
                break;
            case WebqlScopeType.Projection:
                break;
            default:
                break;
        }
    }

    protected override void AnalyzeOperationExpression(WebqlOperationExpression expression)
    {
        /*
         * This ensures that the analysis starts from the leaf nodes of the syntax tree.
         */
        base.AnalyzeOperationExpression(expression);

        switch (expression.GetOperatorCategory())
        {
            case WebqlOperatorCategory.Arithmetic:
            case WebqlOperatorCategory.Relational:
            case WebqlOperatorCategory.StringRelational:
            case WebqlOperatorCategory.Logical:
                AnalyzeBinaryExpression(expression);
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

    private void AnalyzeBinaryExpression(WebqlOperationExpression expression)
    {
        if (expression.Operands.Length != 2)
        {
            throw new SemanticException("Invalid number of operands.", expression);
        }

        var lhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();
        var rhsSemantics = expression.Operands[1].GetSemantics<IExpressionSemantics>();
            
        if (!SemanticsTypeHelper.TypesAreCompatible(lhsSemantics.Type, rhsSemantics.Type))
        {
            throw expression.CreateOperatorIncompatibleTypeException(lhsSemantics.Type, rhsSemantics.Type);
        }
    }

    private void AnalyzeSemanticExpression(WebqlOperationExpression operationExpression)
    {
        operationExpression.EnsureAtLeastOneOperand();

        switch (WebqlOperatorAnalyzer.GetSemanticOperator(operationExpression.Operator))
        {
            case WebqlSemanticOperator.Aggregate:
                break;
            default:
                break;
        }
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

        if(rhsSemantics.Type != typeof(int))
        {
            throw new SemanticException($"Type mismatch: {rhsSemantics.Type} is not an integer type.", expression);
        }
    }

    private void AnalyzeSkipExpression(WebqlOperationExpression expression)
    {
        expression.EnsureOperandCount(2);

        var rhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();

        if(rhsSemantics.Type != typeof(int))
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

