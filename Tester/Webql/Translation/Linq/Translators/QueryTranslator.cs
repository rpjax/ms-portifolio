using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Exceptions;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Translators;

public static class QueryTranslator
{
    public static Expression TranslateQuery(WebqlQuery node)
    {
        /*
         * Outputs a lambda expression that executes the query.
         */
        var translationContext = node.GetTranslationContext();

        if(node.Expression == null)
        {
            throw new TranslationException("Query must have an expression", node);
        }   

        var parameterExpression = translationContext.GetSourceParameterExpression();
        var bodyExpression = SyntaxNodeTranslator.TranslateNode(node.Expression);

        var lambdaExpression = Expression.Lambda(bodyExpression, parameterExpression);
        return lambdaExpression;
    }
}

