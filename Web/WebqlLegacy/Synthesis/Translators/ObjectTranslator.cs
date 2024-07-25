using ModularSystem.Web.Webql.Synthesis.Productions;
using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Translates the &lt;object&gt; symbol to an Expression.
/// </summary>
public class ObjectTranslator
{
    private TranslationOptions Options { get; }
    private ExpressionTranslator ExpressionTranslator { get; }

    public ObjectTranslator(TranslationOptions options)
    {
        Options = options;
        ExpressionTranslator = new ExpressionTranslator(options);
    }

    /// <summary>
    /// EBNF Production: <br/>
    /// object = "{" , [ expr , { "," , expr } ] , "}";
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateObject(TranslationContextOld context, ObjectNode node)
    {
        context = context.CreateTranslationContext(new ObjectProduction());

        var expression = null as Expression;

        foreach (var expressionNode in node.Expressions)
        {
            expression = ExpressionTranslator.TranslateExpression(context, expressionNode);
        }

        if (expression == null)
        {
            throw new TranslationException("", context);
        }

        return expression;
    }

   
}
