using ModularSystem.Web.Webql.Synthesis.Productions;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

public class AxiomTranslator : TranslatorBase
{
    public AxiomTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Production: <br/>
    /// &lt;S&gt; ::= &lt;object&gt;<br/><br/>
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateAxiom(TranslationContext context, ObjectNode node)
    {
        context = context.CreateTranslationContext(new AxiomProduction());

        return new ObjectTranslator(Options)
            .TranslateObject(context, node);
    }

}
