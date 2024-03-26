using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class StatementParser : SyntaxParserBase
{
    public StatementSymbol ParseStatement(ParsingContext context, ObjectPropertyToken property)
    {
        return SyntaxParser.ParseExpression(context, property);
    }
}
