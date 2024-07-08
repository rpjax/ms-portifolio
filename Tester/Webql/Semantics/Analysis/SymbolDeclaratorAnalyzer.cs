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

        if (node.IsRoot())
        {
            DeclareRootSymbols(node);
        }

        else if (node is WebqlOperationExpression operationExpression)
        {
            DeclareOperationSymbols(operationExpression);
        }

        else if(node is WebqlBlockExpression blockExpression)
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
        var localContext = node.GetSemanticContext();
        var queryableType = SemanticContext.CompilationContext.RootQueryableType;
        var elementType = SemanticContext.CompilationContext.RootElementType;
        var accumulatorType = queryableType.MakeGenericType(elementType);

        SetAccumulatorSymbol(node, accumulatorType);
    }

    /// <summary>
    /// Declares the symbolDeclarations for the operation expression node.
    /// </summary>
    /// <param name="node">The operation expression node.</param>
    private void DeclareOperationSymbols(WebqlOperationExpression node)
    {
        var isCollectionOperator = node.IsCollectionOperator();

        if (!isCollectionOperator)
        {
            return;
        }

        if(node.Operands.Length < 1)
        {
            throw new SemanticException("Invalid number of operands.", node);
        }

        var lhsExpression = node.Operands[0];
        var lhsSemantics = lhsExpression.GetSemantics<IExpressionSemantics>();
        var accumulatorType = lhsSemantics.Type;

        if (accumulatorType.IsNotQueryable())
        {
            throw new SemanticException("Left-hand side lhsType must be a queryable lhsType.", node);
        }

        var elementType = accumulatorType.GetQueryableElementType();

        foreach (var operand in node.Operands.Skip(1))
        {
            SetAccumulatorSymbol(operand, elementType);
        }
    }

    private void DeclareBlockExpressionSymbols(WebqlBlockExpression node)
    {
        if(node.GetScopeType() is not WebqlScopeType.Aggregation)
        {
            return;
        }

        var blockContext = node.GetSemanticContext();
        var accumulatorType = blockContext.GetAccumulatorType();

        foreach (var expression in node.Expressions)
        {
            SetAccumulatorSymbol(expression, accumulatorType);

            var expressionSemantics = expression.GetSemantics<IExpressionSemantics>();
            accumulatorType = expressionSemantics.Type;
        }

        blockContext.SetAccumulatorSymbol(accumulatorType);
    }

    private void SetAccumulatorSymbol(WebqlSyntaxNode node, Type type)
    {
        node.GetSemanticContext().SetAccumulatorSymbol(type);
    }

}
