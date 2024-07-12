using Webql.Core.Extensions;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Definitions;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

/// <summary>
/// Represents a visitor for declaring symbolDeclarations in the Webql syntax tree.
/// </summary>
public class SymbolDeclaratorAnalyzer : SyntaxTreeAnalyzer
{
    public SymbolDeclaratorAnalyzer()
    {
     
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
    }

    private void DeclareQuerySymbols(WebqlQuery node)
    {
        if (node.IsNotRoot())
        {
            throw new InvalidOperationException("Query node must be the root node.");
        }

        var compilationContext = node.GetCompilationContext();
        var queryableType = compilationContext.RootQueryableType;
        var elementType = compilationContext.RootElementType;

        var sourceType = queryableType.MakeGenericType(elementType);

        node.DeclareSourceSymbol(sourceType);
    }

    private void DeclareExpressionSymbols(WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Operation:
                DeclareOperationExpressionSymbols((WebqlOperationExpression)node);
                break;
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

        lhsExpression.EnsureIsQueryable();

        var lhsSemantics = lhsExpression.GetSemantics<IExpressionSemantics>();
        var lhsType = lhsSemantics.Type;
        var elementType = lhsType.GetQueryableElementType();

        foreach (var operand in node.Operands.Skip(1))
        {
            operand.DeclareElementSymbol(elementType);
        }
    }

}
