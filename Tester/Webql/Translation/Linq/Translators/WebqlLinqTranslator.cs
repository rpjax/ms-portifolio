using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Analysis;

namespace Webql.Translation.Linq.Translators;

/*
 * The main entry point for the translation of a WebQL query to a LINQ expression.
 */

public static class WebqlLinqTranslator
{
    public static Expression Translate(WebqlQuery node)
    {    
        var contextBinder = new TranslationContextBinderAnalyzer();   
        var expressionDeclarator = new ExpressionDeclaratorAnalyzer();

        /*
         * The translation analysis injects the necessary context into the AST nodes.
         * It also declares the necessary expressions in their respective contexts.
         */
        contextBinder.ExecuteAnalysis(node);
        expressionDeclarator.ExecuteAnalysis(node);
            
        return SyntaxNodeTranslator.TranslateNode(node);
    }
}
