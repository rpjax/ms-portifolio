using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace Webql.Parsing.Ast.Builder.Extensions;

public static class WebqlCstNodeExtensions
{
    const string AccumulatorKey = "accumulator";
    const string DisableBlockSimplificationKey = "disable_block_simplification";
    const string ScopeTypeKey = "scope_type";

    /*
     * accumulator methods.
     */

    public static WebqlExpression GetAccumulatorExpression(this CstNode node)
    {
        var value = node.TryGetProperty(AccumulatorKey);

        if(value is null)
        {
            value = node?.Parent?.GetAccumulatorExpression();
        }

        if(value is null)
        {
            throw new InvalidOperationException();
        }

        if(value is not WebqlExpression expression)
        {
            throw new InvalidOperationException();
        }

        return expression;
    }

    public static void SetAccumulatorExpression(this CstNode node, WebqlExpression expression)
    {
        node.Properties[AccumulatorKey] = expression;
    }

    /*
     * scope type methods.
     */

    public static WebqlScopeType GetScopeType(this CstNode node)
    {
        var value = node.TryGetProperty(ScopeTypeKey);

        if (value is null)
        {
            value = node?.Parent?.GetScopeType();
        }

        if (value is null)
        {
            throw new InvalidOperationException();
        }

        if (value is not WebqlScopeType scopeType)
        {
            throw new InvalidOperationException();
        }

        return scopeType;
    }

    public static void SetScopeType(this CstNode node, WebqlScopeType scopeType)
    {
        node.Properties[ScopeTypeKey] = scopeType;
    }

    /*
     * block simplification methods.
     */

    public static bool IsBlockSimplificationDisabled(this CstNode node)
    {
        return node.GetScopeType() != WebqlScopeType.LogicalFiltering;
    }

}
