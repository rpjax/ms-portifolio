using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ReferenceParser : SyntaxParserBase
{
    public ReferenceExpressionSymbol ParseReference(ParsingContext context, StringToken token)
    {
        return new ReferenceExpressionSymbol(token.Value);
    }
}
