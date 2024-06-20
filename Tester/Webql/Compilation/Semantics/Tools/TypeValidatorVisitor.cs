using ModularSystem.Core.TextAnalysis.Parsing.Components;
using System.Diagnostics.CodeAnalysis;
using Webql.Components;
using Webql.Parsing.Components;
using Webql.Parsing.Tools;
using Webql.Semantics.Components;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Tools;

public class TypeValidatorAnalyzer : SyntaxTreeAnalyzer
{
    private WebqlCompilationContext CompilationContext { get; }

    public TypeValidatorAnalyzer(WebqlCompilationContext compilationContext)
    {
        CompilationContext = compilationContext;
        
    }

    protected override void AnalyzeOperationExpression(WebqlOperationExpression operationExpression)
    {
        base.AnalyzeOperationExpression(operationExpression);

        if (!operationExpression.IsBinary())
        {
            return;
        }

        switch (operationExpression.GetOperatorCategory())
        {
            case WebqlOperatorCategory.Arithmetic:
                AnalyzeArithmeticExpression(operationExpression);
                return;

            case WebqlOperatorCategory.Relational:
                AnalyzeRelationalExpression(operationExpression);
                return;

            case WebqlOperatorCategory.StringRelational:
                AnalyzeStringRelationalExpression(operationExpression);
                return;

            case WebqlOperatorCategory.Logical:
                AnalyzeLogicalExpression(operationExpression);
                return;

            case WebqlOperatorCategory.Semantic:
                AnalyzeSemanticExpression(operationExpression);
                return;

            case WebqlOperatorCategory.CollectionManipulation:
                AnalyzeCollectionManipulationExpression(operationExpression);
                return;

            case WebqlOperatorCategory.CollectionAggregation:
                AnalyzeCollectionAggregationExpression(operationExpression);
                return;

            default:
                throw new InvalidOperationException("Invalid operator category.");
        }
    }

    private void AnalyzeArithmeticExpression(WebqlOperationExpression operationExpression)
    {
        if (operationExpression.Operands.Length != 1)
        {
            throw new Exception("Arithmetic expression must have exactly one operand.");
        }

        var context = operationExpression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        var rhsSemantics = operationExpression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (lhsSemantics.Type != rhsSemantics.Type)
        {
            throw new Exception($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}. At: {operationExpression.Metadata.StartPosition.ToString()}");
        }
    }

    private void AnalyzeRelationalExpression(WebqlOperationExpression operationExpression)
    {
        if (operationExpression.Operands.Length != 1)
        {
            throw new Exception("Relational expression must have exactly one operand.");
        }

        var context = operationExpression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        var rhsSemantics = operationExpression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (lhsSemantics.Type != rhsSemantics.Type)
        {
            throw new Exception($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}. At: {operationExpression.Metadata.StartPosition.ToString()}");
        }
    }

    private void AnalyzeStringRelationalExpression(WebqlOperationExpression operationExpression)
    {
        if (operationExpression.Operands.Length != 1)
        {
            throw new Exception("String relational expression must have exactly one operand.");
        }

        var context = operationExpression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        var rhsSemantics = operationExpression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (lhsSemantics.Type != typeof(string) || rhsSemantics.Type != typeof(string))
        {
            throw new Exception($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}. At: {operationExpression.Metadata.StartPosition.ToString()}");
        }
    }

    private void AnalyzeLogicalExpression(WebqlOperationExpression operationExpression)
    {
        if (operationExpression.Operands.Length != 1)
        {
            throw new Exception("Logical expression must have exactly one operand.");
        }

        var context = operationExpression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        var rhsSemantics = operationExpression.Operands[0].GetSemantics<IExpressionSemantics>();

        if (lhsSemantics.Type != rhsSemantics.Type)
        {
            throw new Exception($"Type mismatch: {lhsSemantics.Type} != {rhsSemantics.Type}. At: {operationExpression.Metadata.StartPosition.ToString()}");
        }
    }

    private void AnalyzeSemanticExpression(WebqlOperationExpression operationExpression)
    {

    }

    private void AnalyzeCollectionManipulationExpression(WebqlOperationExpression operationExpression)
    {
        if (operationExpression.Operands.Length != 1)
        {
            throw new Exception("Collection manipulation expression must have exactly one operand.");
        }

        var context = operationExpression.GetSemanticContext();
        var lhsSemantics = context.GetLeftHandSideSymbol();
        
        if (lhsSemantics.Type.IsNotQueryable())
        {
            throw new SemanticException($"Type mismatch: {lhsSemantics.Type} is not a queryable type.", operationExpression);
        }

        var operatorType = WebqlOperatorClassifier.GetCollectionManipulationOperator(operationExpression.Operator);

        switch (operatorType)
        {
            case WebqlCollectionManipulationOperator.Filter:
                break;

            case WebqlCollectionManipulationOperator.Select:
                break;

            case WebqlCollectionManipulationOperator.SelectMany:
                break;

            case WebqlCollectionManipulationOperator.Limit:
                break;

            case WebqlCollectionManipulationOperator.Skip:
                break;

            default:
                throw new InvalidOperationException("Invalid collection manipulation operator.");
        }
    }

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

