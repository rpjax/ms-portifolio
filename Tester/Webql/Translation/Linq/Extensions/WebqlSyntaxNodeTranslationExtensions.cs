using Webql.Parsing.Ast;
using Webql.Translation.Linq.Attributes;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq.Extensions;

/// <summary>
/// Provides semantic related extensions for the <see cref="WebqlSyntaxNode"/> class.
/// </summary>
public static class WebqlSyntaxNodeTranslationExtensions
{
    public static bool HasTranslationContext(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(AstTranslationAttributes.ContextAttribute);
    }

    public static TranslationContext GetTranslationContext(this WebqlSyntaxNode node)
    {
        var attribute = node.TryGetAttribute<TranslationContext>(AstTranslationAttributes.ContextAttribute);

        if (attribute is null)
        {
            throw new InvalidOperationException();
        }

        return attribute;
    }

    public static void AddTranslationContext(this WebqlSyntaxNode node, TranslationContext context)
    {
        node.AddAttribute(AstTranslationAttributes.ContextAttribute, context);
    }

}
