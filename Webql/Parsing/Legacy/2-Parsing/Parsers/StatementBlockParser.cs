using ModularSystem.Webql.Analysis.DocumentSyntax.Tokenization;
using ModularSystem.Webql.Analysis.Symbols;

namespace ModularSystem.Webql.Analysis.Parsing;

public class StatementBlockParser : SyntaxParserBase
{
    public StatementBlockSymbol ParseStatementBlock(ParsingContext context, ObjectToken token)
    {
        var stmts = new List<StatementSymbol>();

        foreach (var item in token)
        {
            stmts.Add(SyntaxParser.ParseStatement(context, item));
        }

        return new StatementBlockSymbol(stmts.ToArray());
    }
}

