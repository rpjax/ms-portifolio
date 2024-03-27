using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class BinaryArgumentsParser : SyntaxParserBase
{
    public BinaryArgumentsSymbol ParseBinaryArguments(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var left = parser.ParseNextExpression(context);
        var right = parser.ParseNextExpression(context);

        return new BinaryArgumentsSymbol(destination, left, right);
    }
}
