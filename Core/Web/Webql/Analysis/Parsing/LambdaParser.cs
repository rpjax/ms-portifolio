using ModularSystem.Webql.Analysis.Symbols;
using ModularSystem.Webql.Analysis.Tokens;

namespace ModularSystem.Webql.Analysis.Parsing;

public class LambdaParser
{
    public LambdaSymbol ParseLambda(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var args = parser.ParseNextLambdaArguments(context);
        var body = parser.ParseNextObject(context);

        return new LambdaSymbol(args, body);
    }

    public LambdaSymbol ParseUnaryLambda(ParsingContext context, ArrayToken token)
    {
        var parser = new ArrayParser(token);

        var arg = parser.ParseNextUnaryLambdaArguments(context);
        var body = parser.ParseNextObject(context);

        return new LambdaSymbol(arg, body);
    }

}