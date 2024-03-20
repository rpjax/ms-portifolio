using System.Linq.Expressions;

namespace ModularSystem.Webql.Synthesis;

/// <summary>
/// Translates the &lt;param-array&gt; symbol to an Expression array.
/// </summary>
public class LambdaArgsTranslator : TranslatorBase
{
    public LambdaArgsTranslator(TranslationOptions options) : base(options)
    {
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;lambda_args&gt; ::= '[' [ &lt;string&gt; , { ',' &lt;string&gt; } ]  ']'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public ParameterExpression[] TranslateLambdaArgs(TranslationContext context, ArrayNode arrayNode, IEnumerable<Type> types)
    {
        var expressions = new List<ParameterExpression>(arrayNode.Length);
        var nodesArray = arrayNode.Values;
        var typesArray = types.ToArray();

        if (nodesArray.Length != typesArray.Length)
        {
            throw new TranslationException("", context);
        }

        var length = arrayNode.Count();

        for (int i = 0; i < length; i++)
        {
            var node = nodesArray[i];
            var type = typesArray[i];

            var literalNode = TypeCastNode<LiteralNode>(context, node);
            var identifier = literalNode.GetNormalizedValue();

            if (identifier == null)
            {
                throw new TranslationException("", context);
            }

            expressions.Add(Expression.Parameter(type, identifier));
        }

        SetSymbols(context, expressions);
        return expressions.ToArray();
    }

    /// <summary>
    /// Pseudo BNF Production: <br/>
    /// &lt;unary_lambda_args&gt; ::= '[' &lt;string&gt; ']'
    /// </summary>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public ParameterExpression[] TranslateUnaryLambdaArgs(TranslationContext context, ArrayNode arrayNode, Type type)
    {
        return TranslateLambdaArgs(context, arrayNode, new[] { type });
    }

    private void SetSymbols(TranslationContext context, List<ParameterExpression> parameters)
    {
        foreach (var item in parameters)
        {
            context.SetSymbol(item.Name!, item);
        }
    }

}
