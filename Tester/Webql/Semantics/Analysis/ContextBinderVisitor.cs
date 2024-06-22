using Webql.Core;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Context;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Analysis;

public class ContextBinderAnalyzer : SyntaxTreeAnalyzer
{
    private Stack<SemanticContext> ContextStack { get; } 

    public ContextBinderAnalyzer(SemanticContext context)
    {
        ContextStack = new Stack<SemanticContext>();

        //* Push the root context to the stack
        ContextStack.Push(context);
    }

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

        node.AddSemanticContext(localContext);

        ContextStack.Push(childContext);
        base.Analyze(node);
        ContextStack.Pop();
    }
}
