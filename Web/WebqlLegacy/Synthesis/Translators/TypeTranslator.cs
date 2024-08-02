namespace Aidan.Webql.Synthesis;

public class TypeTranslator : TranslatorBase
{
    public TypeTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;type&gt; ::= &lt;string-literal&gt; <br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Type TranslateType(TranslationContextOld context, LiteralNode node)
    {
        var value = node.GetNormalizedValue();

        if(value == null)
        {
            return typeof(void);
        }

        if (value.StartsWith('$'))
        {
            return TranslateReference(context, node).Type;
        }
        else
        {
            return Options.TryGetType(value) 
                ?? throw new TranslationException("", context); ;
        }
    }
}
