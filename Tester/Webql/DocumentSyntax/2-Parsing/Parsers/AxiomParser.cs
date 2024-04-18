using ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Parsing;

public class AxiomParser
{
    public AxiomSymbol ParseAxiom(ParsingContext context, ArrayToken token)
    {
        return new AxiomSymbol(SyntaxParser.ParseLambda(context, token));
    }
}
