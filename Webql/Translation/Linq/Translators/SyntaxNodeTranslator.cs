using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Exceptions;

namespace Webql.Translation.Linq.Translators;

/// <summary>
/// Provides methods to translate a WebqlSyntaxNode into an Expression.
/// </summary>
public static class SyntaxNodeTranslator
{
    /// <summary>
    /// Translates a WebqlSyntaxNode into an Expression.
    /// </summary>
    /// <param name="node">The WebqlSyntaxNode to be translated.</param>
    /// <returns>The translated Expression.</returns>
    /// <exception cref="TranslationException">Thrown when the node type is unknown.</exception>
    public static Expression TranslateNode(WebqlSyntaxNode node)
    {
        switch (node.NodeType)
        {
            case WebqlNodeType.Query:
                return QueryTranslator.TranslateQuery((WebqlQuery)node);

            case WebqlNodeType.Expression:
                return ExpressionTranslator.TranslateExpression((WebqlExpression)node);

            default:
                throw new TranslationException("Unknown node type", node);
        }
    }
}
