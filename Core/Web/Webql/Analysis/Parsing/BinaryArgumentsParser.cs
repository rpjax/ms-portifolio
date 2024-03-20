using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class BinaryArgumentsParser : Parser
{
    public BinaryArgumentsSymbol ParseBinaryArguments(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var destination = parser.ParseNextDestination(context);
        var left = parser.ParseNextArgument(context);
        var right = parser.ParseNextArgument(context);

        return new BinaryArgumentsSymbol(destination, left, right);
    }
}
