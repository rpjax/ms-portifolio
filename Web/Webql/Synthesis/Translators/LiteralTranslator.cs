using ModularSystem.Core;
using System.Linq.Expressions;
using System.Text.Json;

namespace ModularSystem.Webql.Synthesis;

public class LiteralTranslator : TranslatorBase
{
    public LiteralTranslator(TranslationOptions options) : base(options)
    {
    }

    public Expression TranslateLiteral(TranslationContextOld context, LiteralNode node, Type type)
    {
        if(node.Value is null)
        {
            if (!type.ImplementsInterface(typeof(Nullable<>)))
            {
                throw new TranslationException("", context);
            }

            return Expression.Constant(null, type);
        }

        var value = JsonSerializer.Deserialize(node.Value, type, Options.SerializerOptions);

        return Expression.Constant(value, type);
    }

    public Expression TranslateString(TranslationContextOld context, LiteralNode node)
    {
        return TranslateLiteral(context, node, typeof(string));
    }

    public Expression TranslateInt32(TranslationContextOld context, LiteralNode node)
    {
        return TranslateLiteral(context, node, typeof(int));
    }

}
