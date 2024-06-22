using System.Linq.Expressions;
using Webql.Core;
using Webql.Parsing;
using Webql.Semantics.Analysis;
using Webql.Semantics.Context;
using Webql.Translation.Linq;

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
        var translationContext = new TranslationContext(CompilationContext);

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
