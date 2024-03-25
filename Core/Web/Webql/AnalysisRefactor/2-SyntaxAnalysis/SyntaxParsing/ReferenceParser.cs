using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ReferenceParser : SyntaxParser
{
    public ReferenceSymbol ParseReference(ParsingContext context, StringToken token)
    {
        return new ReferenceSymbol(token.Value);
    }
}
