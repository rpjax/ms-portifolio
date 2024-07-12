using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Extensions;
using Webql.Semantics.Scope;

namespace Webql.Semantics.Analysis;

/// <summary>
/// Represents a semantic context binder analyzer.
/// </summary>
public class ScopeBinderAnalyzer : SyntaxTreeAnalyzer
{
    public ScopeBinderAnalyzer()
    {
      
    }

    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                BindRootScope((WebqlQuery)node);
                break;

            case WebqlNodeType.Expression:
                BindExpressionScope((WebqlExpression)node);
                break;
        }

        base.Analyze(node);
    }

    private void BindRootScope(WebqlQuery node)
    {
        if (!node.IsRoot())
        {
            throw new InvalidOperationException("Invalid query node.");
        }

        var rootScope = new WebqlScope(
            parent: null,
            type: WebqlScopeType.Aggregation
        );

        node.BindScope(rootScope);
    }

    private void BindExpressionScope(WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Block:
                BindBlockScope((WebqlBlockExpression)node);
                break;

            case WebqlExpressionType.Operation:
                BindOperationScope((WebqlOperationExpression)node);
                break;
        }
    }

    private void BindBlockScope(WebqlBlockExpression node)
    {
        var localScope = node.GetScope();
        var localScopeType = localScope.Type;

        if (!node.HasScopeAttribute())
        {
            node.BindScope(localScope.CreateChildScope(localScopeType));
        }
    }

    private void BindOperationScope(WebqlOperationExpression node)
    {
        var localScope = node.GetScope();

        var scopeType = node.Operator is WebqlOperatorType.Aggregate
            ? WebqlScopeType.Aggregation
            : localScope.Type;

        if (!node.HasScopeAttribute())
        {
            node.BindScope(localScope.CreateChildScope(localScope.Type));
        }

        foreach (var operand in node.Operands)
        {
            operand.BindScope(localScope.CreateChildScope(scopeType));
        }
    }

}
