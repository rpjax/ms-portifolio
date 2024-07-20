using System.Linq.Expressions;
using Webql.Core;
using Webql.Parsing;
using Webql.Parsing.Ast;
using Webql.Semantics.Analysis;
using Webql.Translation.Linq.Translators;

namespace Webql;

/// <summary>
/// Represents a compiler for Webql queries.
/// </summary>
public class WebqlCompiler
{
    private WebqlCompilerSettings Settings { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebqlCompiler"/> class.
    /// </summary>
    /// <param name="settings">The compiler settings.</param>
    public WebqlCompiler(WebqlCompilerSettings? settings = null)
    {
        Settings = settings ?? new WebqlCompilerSettings();
    }

    /// <summary>
    /// Compiles the specified Webql query into an expression.
    /// </summary>
    /// <param name="query">The Webql query.</param>
    /// <returns>The compiled expression.</returns>
    public Expression Compile(string query, Type elementType)
    {
        // Represents the compilation process. Each compilation process has its own context.
        var context = new WebqlCompilationContext(Settings, elementType);

        /*
         * Analysis.
         */
            
        // Parses the raw query into an AST.
        var syntaxTree = WebqlParser.ParseToAst(query) as WebqlSyntaxNode;

        // Performs the initial semantic analysis, which binds semantics to the AST.
        SemanticAnalyzer.BindSemanticsToAst(context, syntaxTree);

        // Executes the analysis pipeline. It may include tree transformations and other analysis steps.
        SemanticAnalyzer.ExecuteSemanticalAnalysis(ref syntaxTree);

        /*
         * Synthesis.
         */

        // Translates the AST to a LINQ expression.
        return WebqlLinqTranslator.Translate(node: syntaxTree);
    }

}
