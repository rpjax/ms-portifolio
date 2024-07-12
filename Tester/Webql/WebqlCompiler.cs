using System.Linq.Expressions;
using Webql.Core;
using Webql.Parsing;
using Webql.Semantics.Analysis;
using Webql.Translation.Linq.Translators;

namespace Webql;

public class WebqlCompiler
{
    private WebqlCompilerSettings Settings { get; }
        
    public WebqlCompiler(WebqlCompilerSettings settings)
    {
        Settings = settings;   
    }

    public Expression Translate(string query)
    {
        var context = new WebqlCompilationContext(Settings);

        //* ANALYSIS *//

        /*
         * Represents the parse phase of the compiler.
         */
        var syntaxTree = WebqlParser.ParseToAst(query);

        /*
         * Represents the semantic analysis phase of the compiler.
         */
        syntaxTree = SemanticAnalyzer.ExecuteAnalysisPipeline(context, syntaxTree);

        //* SYNTEHESIS *//

        /*
         * Represents the translation phase of the compiler. 
         */
        var translatedExpression = WebqlLinqTranslator.Translate(node: syntaxTree);

        return translatedExpression;
    }
}
