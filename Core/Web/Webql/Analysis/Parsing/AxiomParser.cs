using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class AxiomParser
{
    public AxiomSymbol ParseAxiom(ParsingContext context, ArrayToken token)
    {
        return new AxiomSymbol(TokenParser.ParseUnaryLambda(context, token));
    }
}
