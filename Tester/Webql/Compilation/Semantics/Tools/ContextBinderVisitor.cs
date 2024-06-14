using System.Diagnostics.CodeAnalysis;
using Webql.Parsing.Components;
using Webql.Parsing.Tools;
using Webql.Semantics.Components;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Tools;

/// <summary>
/// Represents a visitor for binding semantic context to syntax nodes.
/// </summary>
public class ContextBinderVisitor : SyntaxNodeVisitor
{
    private Stack<SemanticContext> ContextStack { get; } = new Stack<SemanticContext>();

    /// <summary>
    /// Initializes a new instance of the <see cref="ContextBinderVisitor"/> class.
    /// </summary>
    public ContextBinderVisitor()
    {
        ContextStack.Push(SemanticContext.CreateRootContext());
    }

    /// <inheritdoc/>
    [return: NotNullIfNotNull("node")]
    public override WebqlSyntaxNode? Visit(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return null;
        }

        var context = node.IsScopeSource()
            ? ContextStack.Peek().CreateSubContext()
            : ContextStack.Peek();

        node.AddSemanticContext(context);

        ContextStack.Push(context);
        node = base.Visit(node);
        ContextStack.Pop();

        return node;
    }
}
