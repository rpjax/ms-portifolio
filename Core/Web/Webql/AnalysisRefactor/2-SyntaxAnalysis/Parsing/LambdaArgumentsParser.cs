using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class LambdaArgumentsParser : SyntaxParserBase
{
    public LambdaArgumentSymbol[] ParseLambdaArguments(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);
        var args = new List<LambdaArgumentSymbol>();

        while (parser.TokensCount > 0)
        {
            var strToken = parser.ConsumeNextStringToken(context);
            var identifier = strToken.Value;
            var arg = new LambdaArgumentSymbol(string.Empty, identifier);

            args.Add(arg);
        }

        return args.ToArray();
    }

    public LambdaArgumentSymbol[] ParseUnaryLambdaArguments(ParsingContext context, ArrayToken token)
    {
        if(token.Values.Length != 1)
        {
            throw new ParsingException("", context);
        }

        return ParseLambdaArguments(context, token);
    }

}
