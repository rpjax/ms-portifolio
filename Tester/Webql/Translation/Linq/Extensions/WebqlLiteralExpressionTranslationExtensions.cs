using Webql.Parsing.Ast;

namespace Webql.Translation.Linq.Extensions;

public static class WebqlLiteralExpressionTranslationExtensions
{
    public static string GetNormalizedStringValue(this WebqlLiteralExpression node)
    {
        return node.Value.Substring(1, node.Value.Length - 2);
    }

}
