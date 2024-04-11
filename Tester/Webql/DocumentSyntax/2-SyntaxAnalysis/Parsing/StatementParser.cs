using ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Parsing;

public class StatementParser : SyntaxParserBase
{
    public StatementSymbol ParseStatement(ParsingContext context, ObjectPropertyToken property)
    {
        return SyntaxParser.ParseExpression(context, property);
    }
}
