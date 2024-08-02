using Aidan.Web.Webql.Synthesis.Productions;
using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis;

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
    public Expression TranslateAxiom(TranslationContextOld context, ObjectNode node)
    {
        context = context.CreateTranslationContext(new AxiomProduction());

        return new ObjectTranslator(Options)
            .TranslateObject(context, node);
    }

}
