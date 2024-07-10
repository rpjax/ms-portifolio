namespace Webql.Parsing.Ast.Extensions;

public static class WebqSyntaxNodeExtensions
{
    const string ScopeTypeKey = "scope_type";

    /*
     * Scope type related methods
     */

    public static WebqlScopeType GetScopeType(this WebqlSyntaxNode node)
    {
        if (!node.TryGetAttribute<WebqlScopeType>(ScopeTypeKey, out var scopeType))
        {
            throw new InvalidOperationException("Scope type not found");
        }

        return scopeType;
    }

    public static void SetScopeType(this WebqlSyntaxNode node, WebqlScopeType scopeType)
    {
        node.Attributes[ScopeTypeKey] = scopeType;
    }

}
