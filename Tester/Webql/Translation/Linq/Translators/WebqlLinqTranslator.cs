using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Translators;

/*
 * The main entry point for the translation of a WebQL query to a LINQ expression.
 */

public static class WebqlLinqTranslator
{
    public static Expression Translate(TranslationContext context, WebqlQuery node)
    {    
        var contextBinder = new TranslatorContextBinderVisitor(context);
        var expressionDeclarator = new ExpressionDeclaratorVisitor(context);

        contextBinder.ExecuteAnalysis(node);
        expressionDeclarator.ExecuteAnalysis(node);
            
        return SyntaxNodeTranslator.TranslateNode(node);
    }
}
