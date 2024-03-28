using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class LiteralExpressionParser : SyntaxParserBase
{
    public LiteralExpressionSymbol ParseLiteral(ParsingContext context, ValueToken token)
    {
        switch (token.ValueType)
        {
            case Tokens.ValueType.Null:
                return new NullSymbol();

            case Tokens.ValueType.String:
                return new StringSymbol(((StringToken)token).Value);

            case Tokens.ValueType.Bool:
                return new BoolSymbol(((BoolToken)token).Value);

            case Tokens.ValueType.Number:
                return new NumberSymbol(((NumberToken)token).Value);

            default:
                break;
        }

        throw new ParsingException("", context);
    }
}
