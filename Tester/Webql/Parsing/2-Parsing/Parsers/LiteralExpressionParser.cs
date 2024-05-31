using ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Parsing;

public class LiteralExpressionParser : SyntaxParserBase
{
    public LiteralExpressionSymbol ParseLiteral(ParsingContext context, ValueToken token)
    {
        switch (token.ValueType)
        {
            case DocumentSyntax.Tokenization.ValueType.Null:
                return new NullSymbol();

            case DocumentSyntax.Tokenization.ValueType.String:
                return new StringSymbol(((StringToken)token).Value);

            case DocumentSyntax.Tokenization.ValueType.Bool:
                return new BoolSymbol(((BoolToken)token).Value);

            case DocumentSyntax.Tokenization.ValueType.Number:
                return new NumberSymbol(((NumberToken)token).Value);

            default:
                break;
        }

        throw new ParsingException("", context);
    }
}
