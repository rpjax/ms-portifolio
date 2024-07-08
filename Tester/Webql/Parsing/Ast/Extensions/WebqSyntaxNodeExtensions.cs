namespace Webql.Parsing.Ast.Extensions;

public static class WebqSyntaxNodeExtensions
{
    const string ScopeTypeKey = "scope_type";

    /*
     * Scope type related methods
     */

    public static WebqlScopeType GetScopeType(this WebqlSyntaxNode node)
    {
        var value = node.TryGetAttribute(ScopeTypeKey);

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

    public static void SetScopeType(this WebqlSyntaxNode node, WebqlScopeType scopeType)
    {
        node.Attributes[ScopeTypeKey] = scopeType;
    }

}
