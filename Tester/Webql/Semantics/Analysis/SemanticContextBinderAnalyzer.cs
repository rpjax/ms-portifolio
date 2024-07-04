using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

/// <summary>
/// Represents a semantic context binder analyzer.
/// </summary>
public class SemanticContextBinderAnalyzer : SyntaxTreeAnalyzer
{
    private Stack<SemanticContext> ContextStack { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticContextBinderAnalyzer"/> class.
    /// </summary>
    /// <param name="context">The semantic context.</param>
    public SemanticContextBinderAnalyzer(SemanticContext context)
    {
        ContextStack = new Stack<SemanticContext>();

        //* Push the root context to the stack
        ContextStack.Push(context);
    }

    /// <summary>
    /// Analyzes the given syntax tree node.
    /// </summary>
    /// <param name="node">The syntax tree node to analyze.</param>
    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        var localContext = ContextStack.Peek();
        var childContext = ContextStack.Peek();

        if (node.IsScopeSource())
        {
            childContext = localContext.CreateSubContext();
        }

        if(!node.HasSemanticContext())
        {
            node.AddSemanticContext(localContext);
        }

        ContextStack.Push(childContext);
        base.Analyze(node);
        ContextStack.Pop();
    }
}
