using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class LambdaArgumentsParser : SyntaxParser
{
    public LambdaArgumentsSymbol ParseLambdaArguments(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);
        var args = new List<string>();

        while (parser.TokensCount > 0)
        {
            var strToken = parser.ConsumeNextStringToken(context);
            var identifier = strToken.Value;

            args.Add(identifier);
        }

        return new LambdaArgumentsSymbol(args.ToArray());
    }

    public LambdaArgumentsSymbol ParseUnaryLambdaArguments(ParsingContext context, ArrayToken token)
    {
        if(token.Values.Length != 1)
        {
            throw new ParsingException("", context);
        }

        return ParseLambdaArguments(context, token);
    }

}
