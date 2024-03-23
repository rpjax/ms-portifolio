using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class DestinationParser : Parser
{
    public DestinationSymbol ParseDestination(ParsingContext context, ValueToken token)
    {
        if(token is NullToken nullToken)
        {
            return new DestinationSymbol(null);
        }
        if(token is StringToken stringToken)
        {
            return new DestinationSymbol(stringToken.Value);
        }

        throw new ParsingException("", context);
    }
}
