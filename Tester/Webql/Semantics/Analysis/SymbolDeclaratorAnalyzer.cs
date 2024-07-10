using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Definitions;
using Webql.Semantics.Exceptions;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

/// <summary>
/// Represents a visitor for declaring symbolDeclarations in the Webql syntax tree.
/// </summary>
public class SymbolDeclaratorAnalyzer : SyntaxTreeAnalyzer
{
    private SemanticContext SemanticContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SymbolDeclaratorAnalyzer"/> class.
    /// </summary>
    /// <param name="context">The root semantic localContext.</param>
    public SymbolDeclaratorAnalyzer(SemanticContext context)
    {
        SemanticContext = context;
    }

    /// <inheritdoc/>
    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                DeclareQuerySymbols((WebqlQuery)node);
                break;

            case WebqlNodeType.Expression:
                DeclareExpressionSymbols((WebqlExpression)node);
                break;
        }

        base.Analyze(node);
        return;

        if (node.IsRoot())
        {
            DeclareRootSymbols(node);
        }

        else if (node is WebqlOperationExpression operationExpression)
        {
            DeclareOperationExpressionSymbols(operationExpression);
        }

        else if (node is WebqlBlockExpression blockExpression)
        {
            DeclareBlockExpressionSymbols(blockExpression);
        }

        base.Analyze(node);
    }

    /// <summary>
    /// Declares the symbolDeclarations for the root node.
    /// </summary>
    /// <param name="node">The root node.</param>
    private void DeclareRootSymbols(WebqlSyntaxNode node)
    {
        var queryableType = SemanticContext.CompilationContext.RootQueryableType;
        var elementType = SemanticContext.CompilationContext.RootElementType;
        var accumulatorType = queryableType.MakeGenericType(elementType);

        node.DeclareAccumulatorSymbol(accumulatorType);
    }

    private void DeclareQuerySymbols(WebqlQuery node)
    {
        if (node.IsNotRoot())
        {
            throw new InvalidOperationException("Query node must be the root node.");
        }

        var queryableType = SemanticContext.CompilationContext.RootQueryableType;
        var elementType = SemanticContext.CompilationContext.RootElementType;
        var accumulatorType = queryableType.MakeGenericType(elementType);

        node.DeclareAccumulatorSymbol(accumulatorType);
    }

    private void DeclareExpressionSymbols(WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Block:
                DeclareBlockExpressionSymbols((WebqlBlockExpression)node);
                break;

            case WebqlExpressionType.Operation:
                DeclareOperationExpressionSymbols((WebqlOperationExpression)node);
                break;
        }
    }

    private void DeclareBlockExpressionSymbols(WebqlBlockExpression node)
    {
        switch (node.GetScopeType())
        {
            case WebqlScopeType.Aggregation:
                DeclareAggregationBlockSymbols(node);
                break;
        }
    }

    private void DeclareAggregationBlockSymbols(WebqlBlockExpression node)
    {
        var accumulatorType = node.GetAccumulatorType();

        foreach (var expression in node.Expressions)
        {
            expression.DeclareAccumulatorSymbol(accumulatorType);

            var expressionSemantics = expression.GetSemantics<IExpressionSemantics>();

            accumulatorType = expressionSemantics.Type;
        }
    }

    /// <summary>
    /// Declares the symbolDeclarations for the operation expression node.
    /// </summary>
    /// <param name="node">The operation expression node.</param>
    private void DeclareOperationExpressionSymbols(WebqlOperationExpression node)
    {
        if (!node.IsCollectionOperator())
        {
            return;
        }

        node.EnsureAtLeastOneOperand();

        var lhsExpression = node.Operands[0];
        var lhsSemantics = lhsExpression.GetSemantics<IExpressionSemantics>();
        var lhsType = lhsSemantics.Type;

        lhsExpression.EnsureIsQueryable();

        var elementType = lhsType.GetQueryableElementType();

        foreach (var operand in node.Operands.Skip(1))
        {
            operand.DeclareAccumulatorSymbol(elementType);
        }
    }

}
