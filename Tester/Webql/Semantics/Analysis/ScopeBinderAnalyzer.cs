using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
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
            parent: null
        );

        node.BindScope(rootScope);
    }

    private void BindExpressionScope(WebqlExpression node)
    {
        switch (node.ExpressionType)
        {
            case WebqlExpressionType.Operation:
                BindOperationScope((WebqlOperationExpression)node);
                break;
        }
    }

    private void BindOperationScope(WebqlOperationExpression node)
    {
        var localScope = node.GetScope();

        if (!node.HasScopeAttribute())
        {
            node.BindScope(localScope.CreateChildScope());
        }

        foreach (var operand in node.Operands)
        {
            operand.BindScope(localScope.CreateChildScope());
        }
    }

}
