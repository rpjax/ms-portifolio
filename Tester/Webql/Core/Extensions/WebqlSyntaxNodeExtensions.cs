using System.Runtime.CompilerServices;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;

namespace Webql.Core.Extensions;

/// <summary>
/// Provides extension methods for <see cref="WebqlSyntaxNode"/> instances.
/// </summary>
public static class WebqlSyntaxNodeExtensions
{
    const string CompilationContextKey = "compilation_context";

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T As<T>(this WebqlSyntaxNode node) where T : WebqlSyntaxNode
    {
        if(node is T t)
        {
            return t;
        }

        throw new InvalidOperationException("Attempted to cast a node to an incompatible type during semantic analysis.");
    }

    /*
     * Compilation context related methods
     */

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static WebqlCompilationContext GetCompilationContext(this WebqlSyntaxNode node)
    {
        var current = node;

        while (current is not null)
        {
            if(current.HasAttribute(CompilationContextKey))
            {
                var context = current.GetAttribute<WebqlCompilationContext>(CompilationContextKey);

                if(context is null)
                {
                    throw new InvalidOperationException("The compilation context attribute is not of the expected type.");
                }

                return context;
            }

            current = current.Parent;
        }

        throw new InvalidOperationException("The compilation context attribute was not found in the node hierarchy.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void BindCompilationContext(this WebqlSyntaxNode node, WebqlCompilationContext context)
    {
        if (!node.IsRoot())
        {
            throw new InvalidOperationException("The compilation context can only be set on the root node.");
        }

        node.SetAttribute(CompilationContextKey, context);
    }

}
