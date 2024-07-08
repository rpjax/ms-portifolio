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
    private SemanticContext RootContext { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SemanticContextBinderAnalyzer"/> class.
    /// </summary>
    /// <param name="context">The semantic context.</param>
    public SemanticContextBinderAnalyzer(SemanticContext context)
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

        if (node.IsRoot())
        {
            BindRootContext(node);
        }
        else
        {
            BindLocalContext(node);
        }

        base.Analyze(node);
    }

    private void BindRootContext(WebqlSyntaxNode node)
    {
        node.BindSemanticContext(RootContext);
    }

    private void BindLocalContext(WebqlSyntaxNode node)
    {
        if (node.HasSemanticContext())
        {
            return;
        }
        if (node.Parent is null)
        {
            throw new InvalidOperationException();
        }

        var parentContext = node.Parent.TryGetClosestSemanticContext();

        if(parentContext is null)
        {
            throw new InvalidOperationException();
        }
            
        var localContext = parentContext.CreateChildContext();

        node.BindSemanticContext(localContext);        
    }

}
