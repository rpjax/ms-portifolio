using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class LambdaArgumentsParser : Parser
{
    public LambdaArgumentsSymbol ParseLambdaArguments(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);
        var args = new List<string>();

        while (parser.TokensCount > 0)
        {
            args.Add(parser.ConsumeNextStringToken(context).Value);
        }

        return new LambdaArgumentsSymbol(args.ToArray());
    }

    public LambdaArgumentsSymbol ParseUnaryLambdaArguments(ParsingContext context, ArrayToken token)
    {
        if(token.Count() != 1)
        {
            throw new ParsingException("", context);
        }

        return ParseLambdaArguments(context, token);
    }

}
