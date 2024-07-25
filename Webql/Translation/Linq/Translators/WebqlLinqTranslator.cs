using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Analysis;

namespace Webql.Translation.Linq.Translators;

/// <summary>
/// The main entry point for the translation of a WebQL query to a LINQ expression.
/// </summary>
public static class WebqlLinqTranslator
{
    /// <summary>
    /// Translates the given WebQL syntax node to a LINQ expression.
    /// </summary>
    /// <param name="node">The WebQL syntax node to translate.</param>
    /// <returns>The translated LINQ expression.</returns>
    public static Expression Translate(WebqlSyntaxNode node)
    {
        InjectTranslationComponents(node);
        return SyntaxNodeTranslator.TranslateNode(node);
    }

    /// <summary>
    /// Translates the given WebQL query to a LINQ expression.
    /// </summary>
    /// <param name="node">The WebQL syntax node to translate.</param>
    /// <returns>The translated LINQ expression.</returns>
    public static LambdaExpression TranslateQuery(WebqlQuery node)
    {
        InjectTranslationComponents(node);
        return QueryTranslator.TranslateQuery(node);
    }

    /// <summary>
    /// Injects the necessary translation components into the WebQL syntax node.
    /// </summary>
    /// <param name="node">The WebQL syntax node to inject components into.</param>
    public static void InjectTranslationComponents(WebqlSyntaxNode node)
    {
        /*
         * The translation analysis injects the necessary context into the AST nodes.
         * It also declares the necessary expressions in their respective contexts.
         */

        var contextBinder = new TranslationContextBinderAnalyzer();
        var expressionDeclarator = new ExpressionDeclaratorAnalyzer();

        contextBinder.ExecuteAnalysis(node);
        expressionDeclarator.ExecuteAnalysis(node);
    }
}
