using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Semantics.Extensions;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Exceptions;
using Webql.Translation.Linq.Extensions;

namespace Webql.Translation.Linq.Translators;

public static class QueryTranslator
{
    public static Expression TranslateQuery(TranslationContext context, WebqlQuery node)
    {
        /*
         * Outputs a lambda expression that executes the query.
         */
        var semanticContext = node.GetSemanticContext();
        var lhsSemantics = semanticContext.GetLeftHandSideSymbol();

        var translationContext = node.GetTranslationContext();

        var parameterType = lhsSemantics.Type;
        var parameterName = lhsSemantics.Identifier;
        var parameter = Expression.Parameter(parameterType, parameterName);
        parameter = translationContext.GetLeftHandSideExpression<ParameterExpression>(); 

        if (node.Expression is null)
        {
            throw new TranslationException("Query expression is null.", context);
        }

        var body = ExpressionTranslator.TranslateExpression(context, node.Expression);

        throw new NotImplementedException();
    }
}

