using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Exceptions;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Translators;

public static class QueryTranslator
{
    public static LambdaExpression TranslateQuery(WebqlQuery node)
    {
        /*
         * Outputs a lambda expression that executes the query.
         */

        if(node.Expression == null)
        {
            throw new TranslationException("Query must have an expression", node);
        }   

        var bodyExpression = ExpressionTranslator.TranslateExpression(node.Expression);
        var parameterExpression = node.GetSourceParameterExpression();

        return Expression.Lambda(bodyExpression, parameterExpression);
    }
}

