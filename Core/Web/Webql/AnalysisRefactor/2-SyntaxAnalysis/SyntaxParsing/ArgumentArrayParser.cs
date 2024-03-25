using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class ArgumentArrayParser
{
    public ArgumentSymbol[] ParseArgumentArray(ParsingContext context, ArrayToken token)
    {
        var args = new List<ArgumentSymbol>(token.Values.Length);

        foreach (var argToken in token)
        {
            args.Add(TokenParser.ParseArgument(context, argToken));
        }

        return args.ToArray();
    }
}
