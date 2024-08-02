using System.Linq.Expressions;

namespace Aidan.Webql.Synthesis;

public class ExpressionTranslator : TranslatorBase
{
    /// <summary>
    /// Provides access to translation options and configurations.
    /// </summary>
    private TranslationOptions Options { get; }

    /// <summary>
    /// Manages the translation of individual operators within nodes.
    /// </summary>
    private OperatorsTranslator OperatorsTranslator { get; }

    public ExpressionTranslator(TranslationOptions options) : base(options)
    {
        Options = options;
        OperatorsTranslator = new OperatorsTranslator(options);
    }

    /// <summary>
    /// EBNF Production: <br/>
    /// <code>
    /// expr = <br/>
    ///     arithmetic_expr | <br/>
    ///     relational_expr | <br/>
    ///     pattern_expr | <br/>
    ///     logical_expr | <br/>
    ///     semantic_expr | <br/>
    ///     query_expr | <br/>
    ///     aggregation_expr;
    /// </code>
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <exception cref="TranslationException"></exception>
    public Expression TranslateExpression(TranslationContextOld context, ExpressionNode node)
    {
        var lhs = node.Lhs.Value;
        var rhs = node.Rhs.Value;

        var op = WebqlHelper.ParseOperatorString(context, lhs);
        var opArgs = TypeCastNode<ArrayNode>(context, rhs);

        return OperatorsTranslator.Translate(context, opArgs, op);
    }
}
