using Webql.Core;
using Webql.Core.Extensions;
using Webql.Parsing.Analysis;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Analysis;

public class TranslationContextBinderAnalyzer : SyntaxTreeAnalyzer
{
    public TranslationContextBinderAnalyzer()
    {

    }

    protected override void Analyze(WebqlSyntaxNode? node)
    {
        if (node is null)
        {
            return;
        }

        if (node.IsRoot())
        {
            var rootContext = new TranslationContext(
                compilationContext: node.GetCompilationContext(),
                parent: null,
                cache: new()
            );

            node.BindTranslationContext(rootContext);
            base.Analyze(node);
            return;
        }

        if (node.HasTranslationContextAttribute())
        {
            base.Analyze(node);
            return;
        }

        if (!node.HasScopeAttribute())
        {
            base.Analyze(node);
            return;
        }

        var localContext = node.GetTranslationContext();
        var childContext = localContext.CreateChildContext();

        node.BindTranslationContext(childContext);
        base.Analyze(node);
    }

}
