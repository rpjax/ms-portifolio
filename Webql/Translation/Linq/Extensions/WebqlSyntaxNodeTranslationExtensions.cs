using System.Linq.Expressions;
using Webql.Parsing.Ast;
using Webql.Translation.Linq.Attributes;
using Webql.Translation.Linq.Context;

namespace Webql.Translation.Linq.Extensions;

/// <summary>
/// Provides semantic related extensions for the <see cref="WebqlSyntaxNode"/> class.
/// </summary>
public static class WebqlSyntaxNodeTranslationExtensions
{
    public static bool HasTranslationContextAttribute(this WebqlSyntaxNode node)
    {
        return node.HasAttribute(TranslationAstAttributes.ContextAttribute);
    }

    public static TranslationContext GetTranslationContext(this WebqlSyntaxNode node)
    {
        var current = node;

        while (current is not null)
        {
            if(current.HasTranslationContextAttribute())
            {
                return current.GetAttribute<TranslationContext>(TranslationAstAttributes.ContextAttribute);
            }

            current = current.Parent;
        }
        
        throw new InvalidOperationException();
    }

    public static void BindTranslationContext(this WebqlSyntaxNode node, TranslationContext context)
    {
        node.AddAttribute(TranslationAstAttributes.ContextAttribute, context);
    }

    /*
     * Symbol resolution extensions.
     */

    public static ParameterExpression GetSourceParameterExpression(this WebqlSyntaxNode node)
    {
        return node.GetTranslationContext().GetSourceParameterExpression();
    }

    public static ParameterExpression GetElementParameterExpression(this WebqlSyntaxNode node)
    {
        return node.GetTranslationContext().GetElementParameterExpression();
    }

}
