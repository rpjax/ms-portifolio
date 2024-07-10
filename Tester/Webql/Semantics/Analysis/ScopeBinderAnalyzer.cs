using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Exceptions;
using Webql.Semantics.Extensions;
using Webql.Semantics.Scope;

namespace Webql.Semantics.Analysis;

/// <summary>
/// Represents a semantic context binder analyzer.
/// </summary>
public class ScopeBinderAnalyzer : SyntaxTreeAnalyzer
{
    private Stack<SemanticContext> ContextStack { get; }
    private SemanticContext RootContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ScopeBinderAnalyzer"/> class.
    /// </summary>
    /// <param name="context">The semantic context.</param>
    public ScopeBinderAnalyzer(SemanticContext context)
    {
        ContextStack = new Stack<SemanticContext>();
        RootContext = context;

        //* Push the root context to the stack
        ContextStack.Push(context);
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
                BindQueryScope((WebqlQuery)node);
                break;

            case WebqlNodeType.Expression:
                BindExpressionScope((WebqlExpression)node);
                break;
        }

        base.Analyze(node);
    }

    private void BindQueryScope(WebqlQuery node)
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
        var parentScope = node.GetScope();

        if (!node.HasScopeAttribute())
        {
            switch (node.Operator)
            {
                case WebqlOperatorType.Aggregate:
                    node.BindScope(parentScope.CreateChildScope(WebqlScopeType.Aggregation));
                    break;

                default:
                    node.BindScope(parentScope.CreateChildScope(parentScope.Type));
                    break;
            }
        }

        var localScope = node.GetScope();

        if (!node.IsCollectionOperator())
        {
            return;
        }

        foreach (var operand in node.Operands.Skip(1))
        {
            if (operand.HasScopeAttribute())
            {
                continue;
            }

            operand.BindScope(localScope.CreateChildScope(localScope.Type));
        }
    }

}
