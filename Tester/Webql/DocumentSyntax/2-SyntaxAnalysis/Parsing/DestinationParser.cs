using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class DestinationParser : SyntaxParserBase
{
    public DestinationSymbol ParseDestination(ParsingContext context, ValueToken token)
    {
        if(token is NullToken nullToken)
        {
            return new DestinationSymbol(new NullSymbol());
        }
        if(token is StringToken stringToken)
        {
            return new DestinationSymbol(new StringSymbol(stringToken.Value));
        }

        throw new ParsingException("", context);
    }
}
