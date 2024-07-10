using System.Linq.Expressions;
using Webql.Core;
using Webql.Core.Extensions;
using Webql.Parsing;
using Webql.Semantics.Analysis;
using Webql.Semantics.Context;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Translators;

namespace Webql;

public class WebqlCompiler
{
    private WebqlCompilationContext CompilationContext { get; }
        
    public WebqlCompiler(WebqlCompilerSettings settings)
    {
        CompilationContext = new WebqlCompilationContext(settings);
    }

    public Expression Translate(string query)
    {
        var syntaxTree = WebqlParser.ParseToAst(query);
        var semanticContext = SemanticContext.CreateRootContext(CompilationContext);
        var translationContext = TranslationContext.CreateRootContext(CompilationContext);

        syntaxTree.SetCompilationContext(CompilationContext);
            
        SemanticAnalyzer.ExecuteAnalysisPipeline(
            context: semanticContext, 
            node: syntaxTree
        );

        var translatedExpression = WebqlLinqTranslator.Translate(
            context: translationContext,
            node: syntaxTree
        );

        return translatedExpression;
    }
}
