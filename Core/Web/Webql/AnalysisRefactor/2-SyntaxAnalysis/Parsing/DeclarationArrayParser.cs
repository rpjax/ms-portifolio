using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;
using System.Text.RegularExpressions;

namespace ModularSystem.Webql.Analysis.Parsing;

public class DeclarationArrayParser : SyntaxParserBase
{
    public static Regex DeclarationStringRegex = new Regex("(^[a-zA-Z_][a-zA-Z_0-9]*)+\\s+([a-zA-Z_][a-zA-Z_0-9]*)|(^[a-zA-Z_][a-zA-Z_0-9]*)", RegexOptions.Compiled);

    public DeclarationStatementSymbol ParseDeclaration(ParsingContext context, StringToken token)
    {
        var value = token.Value;

        var matches = DeclarationStringRegex.Matches(value).AsEnumerable().ToArray();

        if(matches.Length != 1)
        {
            throw new Exception();
        }

        var split = value.Split(' ');

        var type = 
            split.Length == 2
            ? split[0]
            : null;

        var identifier =
           split.Length == 2
           ? split[1]
           : split[0];

        return new DeclarationStatementSymbol(
            type: type,
            identifier: identifier,
            modifiers: null
        );
    }

    public DeclarationStatementSymbol[] ParseDeclarationArray(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);
        var statements = new List<DeclarationStatementSymbol>();

        while (parser.TokensCount > 0)
        {
            statements.Add(ParseDeclaration(context, parser.ConsumeNextStringToken(context)));
        }

        return statements.ToArray();
    }

}
