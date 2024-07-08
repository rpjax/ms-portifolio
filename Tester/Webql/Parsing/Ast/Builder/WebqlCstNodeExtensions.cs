using ModularSystem.Core.TextAnalysis.Parsing.Components;

namespace Webql.Parsing.Ast.Builder.Extensions;

public static class WebqlCstNodeExtensions
{
    const string MemberAccessStackKey = "member_access_stack";
    const string DisableBlockSimplificationKey = "disable_block_simplification";
    const string ScopeTypeKey = "scope_type";

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
     * member access stack methods.
     */

    public static IEnumerable<string> GetLhsExpressionMemberAccessList(this CstNode node)
    {
        var value = null as IEnumerable<string>;
        var current = node;

        while (current is not null)
        {
            value = current.TryGetProperty(MemberAccessStackKey) as IEnumerable<string>;

            if (value is not null)
            {
                break;
            }

            current = current.Parent;
        }

        if (value is null)
        {
            return Array.Empty<string>();
        }

        if (value is not IEnumerable<string> strs)
        {
            throw new InvalidOperationException();
        }

        return strs;
    }

    public static void AddMemberAccessToLhsExpression(this CstNode node, string memberName)
    {
        var strs = node.GetLhsExpressionMemberAccessList().ToList();

        strs.Add(memberName);
        
        node.Properties[MemberAccessStackKey] = strs;
    }

    public static void ResetLhsExpression(this CstNode node)
    {
        node.Properties[MemberAccessStackKey] = new List<string>();
    }

    public static WebqlExpression GetLhsExpression(this CstNode node)
    {
        var accumulatorReference = new WebqlReferenceExpression(
            metadata: WebqlAstBuilder.TranslateNodeMetadata(node),
            attributes: null,
            identifier: AstHelper.AccumulatorIdentifier
        );
        var expression = accumulatorReference as WebqlExpression;

        var members = node.GetLhsExpressionMemberAccessList();

        foreach (var member in members)
        {
            expression = new WebqlMemberAccessExpression(
                metadata: WebqlAstBuilder.TranslateNodeMetadata(node),
                attributes: null,
                expression: expression,
                memberName: member
            );
        }

        return expression;
    }


    /*
     * block simplification methods.
     */

    public static bool IsBlockSimplificationDisabled(this CstNode node)
    {
        return node.GetScopeType() != WebqlScopeType.LogicalFiltering;
    }

}
