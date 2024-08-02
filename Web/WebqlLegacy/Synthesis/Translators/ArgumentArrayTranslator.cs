using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis;

/// <summary>
/// Translates the &lt;arg-array&gt; symbol to an Expression array.
/// </summary>
public class ArgumentArrayTranslator : TranslatorBase
{
    public ArgumentArrayTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;arg_array&gt; ::= '[' [ &lt;arg&gt; , { ',' &lt;arg&gt; } ] ']'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression[] TranslateArgumentArray(TranslationContextOld context, ArrayNode arrayNode)
    {
        var expressions = new List<Expression>(arrayNode.Length);

        foreach (var node in arrayNode)
        {
            expressions.Add(TranslateArgument(context, node));
        }

        return expressions.ToArray();
    }
}
