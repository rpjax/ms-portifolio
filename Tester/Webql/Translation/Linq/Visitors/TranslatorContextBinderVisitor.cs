using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Translators;

public class TranslatorContextBinderVisitor : SyntaxTreeAnalyzer
{
    private Stack<TranslationContext> ContextStack { get; }

    public TranslatorContextBinderVisitor(TranslationContext context)
    {
        ContextStack = new Stack<TranslationContext>();
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

        node.AddTranslationContext(localContext);

        ContextStack.Push(childContext);
        base.Analyze(node);
        ContextStack.Pop();
    }

}

