using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Context;
using Webql.Translation.Linq.Exceptions;

namespace Webql.Translation.Linq.Translators;

public static class WebqlLinqTranslator
{
    public static Expression Translate(TranslationContext context, WebqlSyntaxNode node)
    {
        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                return QueryTranslator.TranslateQuery(context, (WebqlQuery)node);

            case WebqlNodeType.Expression:
                return ExpressionTranslator.TranslateExpression(context, (WebqlExpression)node);

            default:
                throw new TranslationException("Unknown node type", context);
        }
    }
}
