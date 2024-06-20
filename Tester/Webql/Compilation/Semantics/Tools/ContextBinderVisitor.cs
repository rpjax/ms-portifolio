using Webql.Components;
using Webql.Parsing.Components;
using Webql.Parsing.Tools;
using Webql.Semantics.Components;
using Webql.Semantics.Extensions;

namespace Webql.Semantics.Tools;

public class ContextBinderAnalyzer : SyntaxTreeAnalyzer
{
    private WebqlCompilationContext CompilationContext { get; }
    private Stack<SemanticContext> ContextStack { get; } 

    public ContextBinderAnalyzer(WebqlCompilationContext compilationContext)
    {
        CompilationContext = compilationContext;
        ContextStack = new Stack<SemanticContext>();

        //* Push the root context to the stack
        ContextStack.Push(SemanticContext.CreateRootContext(compilationContext));
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
