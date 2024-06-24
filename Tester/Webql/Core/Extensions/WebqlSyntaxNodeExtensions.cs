using Webql.Parsing.Ast;

namespace Webql.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="WebqlSyntaxNode"/> instances.
/// </summary>
public static class WebqlSyntaxNodeExtensions
{
    public static T As<T>(this WebqlSyntaxNode node) where T : WebqlSyntaxNode
    {
        if(node is T t)
        {
            return t;
        }

        throw new InvalidOperationException("Attempted to cast a node to an incompatible type during semantic analysis.");
    }

}
