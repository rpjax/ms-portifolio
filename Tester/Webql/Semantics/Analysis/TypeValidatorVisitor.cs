using System.Reflection;
using Webql.Core;
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

    protected override void AnalyzeOperationExpression(WebqlOperationExpression expression)
    {
        /*
         * This ensures that the analysis starts from the leaf nodes of the syntax tree.
         */
        base.AnalyzeOperationExpression(expression);

        switch (expression.GetOperatorCategory())
        {
            case WebqlOperatorCategory.Arithmetic:
                AnalyzeArithmeticExpression(expression);
                return;

            case WebqlOperatorCategory.Relational:
                AnalyzeRelationalExpression(expression);
                return;

            case WebqlOperatorCategory.StringRelational:
                AnalyzeStringRelationalExpression(expression);
                return;

            case WebqlOperatorCategory.Logical:
                AnalyzeLogicalExpression(expression);
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

    private void AnalyzeArithmeticExpression(WebqlOperationExpression expression)
    {
        if (expression.Operands.Length != 1)
        {
            throw new SemanticException("Arithmetic expression must have exactly one operand.", expression);
        }
   
        var context = expression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        var rhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (lhsSemantics.Type != rhsSemantics.Type)
        {
            throw new SemanticException($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}.", expression);
        }
    }

    private void AnalyzeRelationalExpression(WebqlOperationExpression expression)
    {
        if (expression.Operands.Length != 1)
        {
            throw new SemanticException("Relational expression must have exactly one operand.", expression);
        }

        var context = expression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        var rhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();
            
        if (!SemanticsTypeHelper.TypesAreCompatible(lhsSemantics.Type, rhsSemantics.Type))
        {
            throw new SemanticException($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}.", expression);
        }
    }

    private void AnalyzeStringRelationalExpression(WebqlOperationExpression expression)
    {
        if (expression.Operands.Length != 1)
        {
            throw new SemanticException("String relational expression must have exactly one operand.", expression);
        }

        var context = expression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        var rhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (lhsSemantics.Type != typeof(string) || rhsSemantics.Type != typeof(string))
        {
            throw new SemanticException($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}.", expression);
        }
    }

    private void AnalyzeLogicalExpression(WebqlOperationExpression expression)
    {
        if (expression.Operands.Length != 1)
        {
            throw new SemanticException("Logical expression must have exactly one operand.", expression);
        }

        var context = expression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        var rhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (lhsSemantics.Type != rhsSemantics.Type)
        {
            throw new SemanticException($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}.", expression);
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
        if (expression.Operands.Length != 1)
        {
            throw new SemanticException("Collection manipulation expression must have exactly one operand.", expression);
        }

        var context = expression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        
        if (lhsSemantics.Type.IsNotQueryable())
        {
            throw new SemanticException($"Type mismatch: {lhsSemantics.Type} is not a queryable type.", expression);
        }

        var operatorType = WebqlOperatorClassifier.GetCollectionManipulationOperator(expression.Operator);

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
        if(expression.Operands.Length != 1)
        {
            throw new SemanticException("Limit expression must have exactly one operand.", expression);
        }

        var rhsSemantics = expression.Operands[0].GetSemantics<IExpressionSemantics>();

        if(rhsSemantics.Type != typeof(int))
        {
            throw new SemanticException($"Type mismatch: {rhsSemantics.Type} is not an integer type.", expression);
        }
    }

    private void AnalyzeSkipExpression(WebqlOperationExpression expression)
    {
        if(expression.Operands.Length != 1)
        {
            throw new SemanticException("Skip expression must have exactly one operand.", expression);
        }

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
        var context = operationExpression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();

        if (lhsSemantics.Type.IsNotQueryable())
        {
            throw new SemanticException($"Type mismatch: {lhsSemantics.Type} is not a queryable type.", operationExpression);
        }
    }

}

